using DEFIANTS.Server.Data;
using DEFIANTS.Server.Models.Entities;
using DEFIANTS.Server.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace DEFIANTS.Server.Services;

public class TorneoService : ITorneoService
{
    private readonly ApplicationDbContext _context;

    public TorneoService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task GenerarBracketsAsync(int torneoId)
    {
        var torneo = await _context.Torneos
            .Include(t => t.Inscripciones).ThenInclude(i => i.Equipo)
            .FirstOrDefaultAsync(t => t.Id == torneoId);

        if (torneo == null) throw new Exception("Torneo no encontrado.");

        var equipos = torneo.Inscripciones
            .Where(i => i.EstadoPago == EstadoPago.Completado)
            .Select(i => i.Equipo)
            .OrderBy(x => Guid.NewGuid()) // Mezclar equipos
            .ToList();

        int numEquipos = equipos.Count;
        if (numEquipos < 2) throw new InvalidOperationException("Se necesitan al menos 2 equipos para generar brackets.");

        // 1. CÁLCULOS DEL BRACKET
        int bracketSize = (int)Math.Pow(2, Math.Ceiling(Math.Log(numEquipos, 2)));
        int numByes = bracketSize - numEquipos;
        int numPartidosRonda1 = (numEquipos - numByes) / 2;
        int totalRondas = (int)Math.Log(bracketSize, 2);

        var equiposConBye = equipos.Take(numByes).ToList();
        var equiposSinBye = equipos.Skip(numByes).ToList();

        var partidosPorRonda = new Dictionary<int, List<Partido>>();
        var todosLosPartidos = new List<Partido>();

        // 2. CREAR LA ESTRUCTURA DE PARTIDOS
        for (int ronda = 1; ronda <= totalRondas; ronda++)
        {
            int partidosEnEstaRonda = bracketSize / (int)Math.Pow(2, ronda);
            partidosPorRonda[ronda] = new List<Partido>();
            for (int i = 0; i < partidosEnEstaRonda; i++)
            {
                var nuevoPartido = new Partido
                {
                    TorneoId = torneoId,
                    Ronda = ronda,
                    IndicePartido = i,
                    Estado = EstadoPartido.Pendiente
                };
                partidosPorRonda[ronda].Add(nuevoPartido);
                todosLosPartidos.Add(nuevoPartido);
            }
        }
        
        _context.Partidos.AddRange(todosLosPartidos);
        await _context.SaveChangesAsync(); // Guardar para obtener IDs

        // 3. VINCULAR LOS PARTIDOS (CREAR EL ÁRBOL)
        foreach (var partido in todosLosPartidos.Where(p => p.Ronda < totalRondas))
        {
            partido.PartidoSiguienteId = partidosPorRonda[partido.Ronda + 1][partido.IndicePartido / 2].Id;
        }

        // 4. ASIGNAR EQUIPOS Y "BYES"
        // Asignar equipos que SÍ juegan en la primera ronda
        for (int i = 0; i < numPartidosRonda1; i++)
        {
            var partido = partidosPorRonda[1][i];
            partido.EquipoA_Id = equiposSinBye[i * 2].Id;
            partido.EquipoB_Id = equiposSinBye[i * 2 + 1].Id;
            partido.Estado = EstadoPartido.Listo;
        }

        // Asignar "Byes" (equipos que avanzan automáticamente)
        for (int i = 0; i < numByes; i++)
        {
            // El partido de primera ronda de un "Bye" es el que está después de los partidos reales
            var partidoRonda1Bye = partidosPorRonda[1][numPartidosRonda1 + i];
            var equipoConBye = equiposConBye[i];
            
            partidoRonda1Bye.EquipoA_Id = equipoConBye.Id;
            partidoRonda1Bye.EquipoGanadorId = equipoConBye.Id;
            partidoRonda1Bye.Estado = EstadoPartido.Walkover;

            // Avanzar al ganador directamente al siguiente partido
            var partidoSiguiente = todosLosPartidos.First(p => p.Id == partidoRonda1Bye.PartidoSiguienteId);
            if (partidoRonda1Bye.IndicePartido % 2 == 0)
            {
                partidoSiguiente.EquipoA_Id = equipoConBye.Id;
            }
            else
            {
                partidoSiguiente.EquipoB_Id = equipoConBye.Id;
            }
        }
        
        // 5. ACTUALIZAR ESTADOS
        // Comprobar si algún partido de la segunda ronda ya está listo
        foreach (var partidoRonda2 in partidosPorRonda.GetValueOrDefault(2, new List<Partido>()))
        {
            if (partidoRonda2.EquipoA_Id.HasValue && partidoRonda2.EquipoB_Id.HasValue)
            {
                partidoRonda2.Estado = EstadoPartido.Listo;
            }
        }

        torneo.Status = EstadoTorneo.EnCurso;
        await _context.SaveChangesAsync();
    }

    public async Task ReportarVictoriaAsync(int partidoId, int equipoGanadorId)
    {
        var partidoActual = await _context.Partidos
            .Include(p => p.PartidoSiguiente)
            .FirstOrDefaultAsync(p => p.Id == partidoId);

        if (partidoActual == null) throw new Exception("Partido no encontrado.");
        if (partidoActual.Estado == EstadoPartido.Finalizado || partidoActual.Estado == EstadoPartido.Walkover) 
            throw new InvalidOperationException("El partido ya fue finalizado.");

        // Validar que el ganador es uno de los dos equipos
        if (equipoGanadorId != partidoActual.EquipoA_Id && equipoGanadorId != partidoActual.EquipoB_Id)
            throw new InvalidOperationException("El equipo ganador no es un participante de este partido.");

        partidoActual.EquipoGanadorId = equipoGanadorId;
        partidoActual.Estado = EstadoPartido.Finalizado;

        // Si es la final, terminar el torneo
        if (partidoActual.PartidoSiguiente == null)
        {
            var torneo = await _context.Torneos.FindAsync(partidoActual.TorneoId);
            if (torneo != null) torneo.Status = EstadoTorneo.Finalizado;
            await _context.SaveChangesAsync();
            return;
        }

        // Avanzar al ganador al siguiente bracket
        var siguientePartido = partidoActual.PartidoSiguiente;
        if (partidoActual.IndicePartido % 2 == 0)
        {
            siguientePartido.EquipoA_Id = equipoGanadorId;
        }
        else
        {
            siguientePartido.EquipoB_Id = equipoGanadorId;
        }

        // Si el siguiente partido ya tiene ambos contendientes, está listo
        if (siguientePartido.EquipoA_Id.HasValue && siguientePartido.EquipoB_Id.HasValue)
        {
            siguientePartido.Estado = EstadoPartido.Listo;
        }

        await _context.SaveChangesAsync();
    }
}

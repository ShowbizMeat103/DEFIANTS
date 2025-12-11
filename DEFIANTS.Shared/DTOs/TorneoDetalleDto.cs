using System;
using System.Collections.Generic;
using DEFIANTS.Shared.Enums; // Asegúrate de tener este using

namespace DEFIANTS.Shared.DTOs;

public class TorneoDetalleDto
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string Status { get; set; } // O EstadoTorneo si prefieres el enum
    public int JuegoId { get; set; }
    public string JuegoNombre { get; set; } // <-- Nuevo
    public int MaxEquipos { get; set; }
    public decimal PrecioInscripcion { get; set; }
    public decimal PrizePool { get; set; }
    public DateTime FechaInicio { get; set; }
    public string CreadorId { get; set; } // Para saber si mostrar controles de admin
    public List<PartidoDto> Partidos { get; set; } = new();
    public List<InscripcionDetalleDto> Inscripciones { get; set; } = new(); // <-- Nuevo: Para listar los equipos
}

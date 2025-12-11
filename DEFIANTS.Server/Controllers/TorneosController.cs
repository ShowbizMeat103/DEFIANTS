using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DEFIANTS.Server.Data;
using DEFIANTS.Server.Models.Entities;
using DEFIANTS.Server.Services;
using DEFIANTS.Shared.DTOs;
using DEFIANTS.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DEFIANTS.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TorneosController : ControllerBase
{
    private readonly ITorneoService _torneoService;
    private readonly ApplicationDbContext _context;

    public TorneosController(ITorneoService torneoService, ApplicationDbContext context)
    {
        _torneoService = torneoService;
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<TorneoResumenDto>>> GetTorneos()
    {
        var torneos = await _context.Torneos
            .Select(t => new TorneoResumenDto
            {
                Id = t.Id,
                Titulo = t.Titulo,
                Status = t.Status.ToString(),
                MaxEquipos = t.MaxEquipos,
                FechaInicio = t.FechaInicio 
            })
            .ToListAsync();
        return Ok(torneos);
    }

    [HttpGet("misinscripciones")]
    public async Task<ActionResult<List<MiInscripcionDto>>> GetMisInscripciones()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var misInscripciones = await _context.Inscripciones
            .Where(i => i.Equipo.Miembros.Any(m => m.PerfilJuego.UsuarioId == userId))
            .Select(i => new MiInscripcionDto
            {
                TorneoId = i.Torneo.Id,
                TorneoTitulo = i.Torneo.Titulo,
                Status = i.Torneo.Status.ToString(),
                NombreEquipo = i.Equipo.Nombre
            })
            .ToListAsync();

        return Ok(misInscripciones);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<TorneoDetalleDto>> GetTorneo(int id)
    {
        var torneoDto = await _context.Torneos
            .Where(t => t.Id == id)
            .Select(t => new TorneoDetalleDto
            {
                Id = t.Id,
                Titulo = t.Titulo,
                Status = t.Status.ToString(),
                JuegoId = t.JuegoId,
                JuegoNombre = t.Juego.Nombre,
                MaxEquipos = t.MaxEquipos,
                PrecioInscripcion = t.PrecioInscripcion,
                PrizePool = t.PrizePool,
                FechaInicio = t.FechaInicio,
                CreadorId = t.CreadorId,
                Partidos = t.Partidos.Select(p => new PartidoDto
                {
                    Id = p.Id,
                    Ronda = p.Ronda,
                    IndicePartido = p.IndicePartido,
                    EquipoAId = p.EquipoA_Id,
                    EquipoANombre = p.EquipoA != null ? p.EquipoA.Nombre : "TBD",
                    EquipoBId = p.EquipoB_Id,
                    EquipoBNombre = p.EquipoB != null ? p.EquipoB.Nombre : "TBD",
                    EquipoGanadorId = p.EquipoGanadorId,
                    Estado = p.Estado
                }).OrderBy(p => p.Ronda).ThenBy(p => p.IndicePartido).ToList(),
                Inscripciones = t.Inscripciones.Select(i => new InscripcionDetalleDto
                {
                    Id = i.Id,
                    EquipoId = i.EquipoId,
                    NombreEquipo = i.Equipo.Nombre,
                    EstadoPago = i.EstadoPago
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (torneoDto == null) return NotFound();
        
        return Ok(torneoDto);
    }

    [HttpGet("{torneoId}/inscripciones")]
    public async Task<ActionResult<List<InscripcionDetalleDto>>> GetInscripciones(int torneoId)
    {
        var torneo = await _context.Torneos.FindAsync(torneoId);
        if (torneo == null) return NotFound("Torneo no encontrado.");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (torneo.CreadorId != userId && !User.IsInRole("Admin"))
        {
            return Forbid("No tienes permiso para ver las inscripciones de este torneo.");
        }

        var inscripciones = await _context.Inscripciones
            .Where(i => i.TorneoId == torneoId)
            .Select(i => new InscripcionDetalleDto
            {
                Id = i.Id,
                EquipoId = i.EquipoId,
                NombreEquipo = i.Equipo.Nombre,
                EstadoPago = i.EstadoPago
            })
            .ToListAsync();

        return Ok(inscripciones);
    }

    [HttpPost]
    public async Task<ActionResult<TorneoDto>> CrearTorneo([FromBody] CrearTorneoDto torneoDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var nuevoTorneo = new Torneo { Titulo = torneoDto.Titulo, JuegoId = torneoDto.JuegoId, MaxEquipos = torneoDto.MaxEquipos, PrecioInscripcion = torneoDto.PrecioInscripcion, PrizePool = torneoDto.PrizePool, FechaInicio = torneoDto.FechaInicio, Status = EstadoTorneo.InscripcionesAbiertas, CreadorId = userId! };
        
        _context.Torneos.Add(nuevoTorneo);
        await _context.SaveChangesAsync();

        var responseDto = new TorneoDto
        {
            Id = nuevoTorneo.Id,
            Titulo = nuevoTorneo.Titulo,
            JuegoId = nuevoTorneo.JuegoId,
            MaxEquipos = nuevoTorneo.MaxEquipos,
            PrecioInscripcion = nuevoTorneo.PrecioInscripcion,
            PrizePool = nuevoTorneo.PrizePool,
            FechaInicio = nuevoTorneo.FechaInicio,
            Status = nuevoTorneo.Status,
            CreadorId = nuevoTorneo.CreadorId
        };

        return StatusCode(StatusCodes.Status201Created, responseDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> ActualizarTorneo(int id, [FromBody] CrearTorneoDto torneoDto)
    {
        var torneo = await _context.Torneos.FindAsync(id);
        if (torneo == null) return NotFound();
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (torneo.CreadorId != userId && !User.IsInRole("Admin")) return Forbid("No tienes permiso para editar este torneo.");
        if (torneo.Status != EstadoTorneo.Borrador && torneo.Status != EstadoTorneo.InscripcionesAbiertas) return BadRequest("No se puede editar un torneo que ya ha comenzado o finalizado.");
        torneo.Titulo = torneoDto.Titulo;
        torneo.JuegoId = torneoDto.JuegoId;
        torneo.MaxEquipos = torneoDto.MaxEquipos;
        torneo.PrecioInscripcion = torneoDto.PrecioInscripcion;
        torneo.PrizePool = torneoDto.PrizePool;
        torneo.FechaInicio = torneoDto.FechaInicio;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelarTorneo(int id)
    {
        var torneo = await _context.Torneos.FindAsync(id);
        if (torneo == null) return NotFound();
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (torneo.CreadorId != userId && !User.IsInRole("Admin")) return Forbid("No tienes permiso para cancelar este torneo.");
        torneo.Status = EstadoTorneo.Cancelado;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{torneoId}/inscripciones")]
    public async Task<IActionResult> InscribirEquipo(int torneoId, [FromBody] InscribirEquipoDto inscripcionDto)
    {
        var torneo = await _context.Torneos.Include(t => t.Inscripciones).FirstOrDefaultAsync(t => t.Id == torneoId);
        if (torneo == null) return NotFound("El torneo no existe.");
        var equipo = await _context.Equipos.FindAsync(inscripcionDto.EquipoId);
        if (equipo == null) return NotFound("El equipo no existe.");
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var perfilJuego = await _context.PerfilesJuego.FirstOrDefaultAsync(p => p.UsuarioId == userId && p.JuegoId == equipo.JuegoId);
        if (perfilJuego == null || equipo.CapitanId != perfilJuego.Id) return Forbid("Solo el capitán del equipo puede realizar la inscripción.");
        if (equipo.JuegoId != torneo.JuegoId) return BadRequest("El juego de tu equipo no coincide con el juego del torneo.");
        if (torneo.Status != EstadoTorneo.InscripcionesAbiertas) return BadRequest("Las inscripciones para este torneo no están abiertas.");
        if (torneo.Inscripciones.Any(i => i.EquipoId == inscripcionDto.EquipoId)) return Conflict("Este equipo ya está inscrito en el torneo.");
        if (torneo.Inscripciones.Count >= torneo.MaxEquipos) return Conflict("El torneo ya ha alcanzado el número máximo de equipos.");
        var nuevaInscripcion = new Models.Entities.Inscripcion { TorneoId = torneoId, EquipoId = inscripcionDto.EquipoId, EstadoPago = EstadoPago.Completado };
        _context.Inscripciones.Add(nuevaInscripcion);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Equipo inscrito correctamente." });
    }

    [HttpDelete("{torneoId}/inscripciones/{inscripcionId}")]
    public async Task<IActionResult> CancelarInscripcion(int torneoId, int inscripcionId)
    {
        var torneo = await _context.Torneos.FindAsync(torneoId);
        if (torneo == null) return NotFound("Torneo no encontrado.");
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (torneo.CreadorId != userId && !User.IsInRole("Admin")) return Forbid("No tienes permiso para gestionar las inscripciones de este torneo.");
        var inscripcion = await _context.Inscripciones.FirstOrDefaultAsync(i => i.Id == inscripcionId && i.TorneoId == torneoId);
        if (inscripcion == null) return NotFound("Inscripción no encontrada para este torneo.");
        _context.Inscripciones.Remove(inscripcion);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    
    [HttpPost("{id}/iniciar")]
    public async Task<IActionResult> IniciarTorneo(int id)
    {
        var torneo = await _context.Torneos.FindAsync(id);
        if (torneo == null) return NotFound();
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (torneo.CreadorId != userId && !User.IsInRole("Admin")) return Forbid("No tienes permiso para iniciar este torneo.");

        try
        {
            await _torneoService.GenerarBracketsAsync(id);
            return Ok(new { mensaje = "Torneo iniciado y brackets generados." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("partidos/{partidoId}/victoria")]
    public async Task<IActionResult> ReportarResultado(int partidoId, [FromBody] int ganadorId)
    {
        var partido = await _context.Partidos.Include(p => p.Torneo).FirstOrDefaultAsync(p => p.Id == partidoId);
        if (partido == null) return NotFound("El partido no existe.");
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (partido.Torneo.CreadorId != userId && !User.IsInRole("Admin")) return Forbid("No tienes permiso para reportar resultados en este torneo.");
        try
        {
            await _torneoService.ReportarVictoriaAsync(partidoId, ganadorId);
            return Ok(new { mensaje = "Resultado registrado y bracket actualizado." });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

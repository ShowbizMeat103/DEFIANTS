using System.Security.Claims;
using DEFIANTS.Server.Data;
using DEFIANTS.Server.Models.Entities;
using DEFIANTS.Server.Models.Enums;
using DEFIANTS.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DEFIANTS.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PartidosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PartidosController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/partidos/mispartidos
    [HttpGet("mispartidos")]
    public async Task<IActionResult> GetMisPartidos()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var misEquiposIds = await _context.MiembrosEquipo
            .Where(m => m.PerfilJuego.UsuarioId == userId)
            .Select(m => m.EquipoId)
            .Distinct()
            .ToListAsync();

        if (!misEquiposIds.Any())
        {
            return Ok(new List<object>()); 
        }

        var misPartidos = await _context.Partidos
            .Where(p => (p.EquipoA_Id.HasValue && misEquiposIds.Contains(p.EquipoA_Id.Value)) ||
                        (p.EquipoB_Id.HasValue && misEquiposIds.Contains(p.EquipoB_Id.Value)))
            .Include(p => p.EquipoA)
            .Include(p => p.EquipoB)
            .Include(p => p.Torneo)
            .Select(p => new
            {
                p.Id,
                TorneoTitulo = p.Torneo.Titulo,
                EquipoA = p.EquipoA != null ? p.EquipoA.Nombre : "TBD",
                EquipoB = p.EquipoB != null ? p.EquipoB.Nombre : "TBD",
                p.Ronda,
                p.Estado,
                p.EquipoGanadorId,
                p.ScoreA,
                p.ScoreB
            })
            .ToListAsync();

        return Ok(misPartidos);
    }

    // GET: api/partidos/5
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPartido(int id)
    {
        var partido = await _context.Partidos
            .Include(p => p.EquipoA)
            .Include(p => p.EquipoB)
            .Include(p => p.Torneo)
            .Select(p => new
            {
                p.Id,
                Torneo = new { p.Torneo.Id, p.Torneo.Titulo },
                p.Ronda,
                p.IndicePartido,
                p.Estado,
                EquipoA = p.EquipoA != null ? new { p.EquipoA.Id, p.EquipoA.Nombre } : null,
                EquipoB = p.EquipoB != null ? new { p.EquipoB.Id, p.EquipoB.Nombre } : null,
                p.ScoreA,
                p.ScoreB,
                p.EquipoGanadorId
            })
            .FirstOrDefaultAsync(p => p.Id == id);

        if (partido == null) return NotFound("Partido no encontrado.");

        return Ok(partido);
    }

    // PUT: api/partidos/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CorregirPartido(int id, [FromBody] CorregirPartidoDto partidoDto)
    {
        var partido = await _context.Partidos.FindAsync(id);
        if (partido == null) return NotFound("Partido no encontrado.");

        partido.ScoreA = partidoDto.ScoreA;
        partido.ScoreB = partidoDto.ScoreB;
        partido.EquipoGanadorId = partidoDto.EquipoGanadorId;

        if (partido.EquipoGanadorId.HasValue)
        {
            partido.Estado = EstadoPartido.Finalizado;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }
}

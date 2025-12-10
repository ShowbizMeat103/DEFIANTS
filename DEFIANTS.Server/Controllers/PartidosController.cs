using System.Security.Claims;
using DEFIANTS.Server.Data;
using DEFIANTS.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq; // Necesario para Select
using DEFIANTS.Shared.Enums; // <-- AÑADIDO

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

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<PartidoDto>>> GetPartidos()
    {
        var partidos = await _context.Partidos
            .Select(p => new PartidoDto
            {
                Id = p.Id,
                Ronda = p.Ronda,
                IndicePartido = p.IndicePartido,
                EquipoAId = p.EquipoA_Id,
                EquipoANombre = p.EquipoA != null ? p.EquipoA.Nombre : "TBD",
                EquipoBId = p.EquipoB_Id,
                EquipoBNombre = p.EquipoB != null ? p.EquipoB.Nombre : "TBD",
                EquipoGanadorId = p.EquipoGanadorId,
                Estado = p.Estado // <-- YA ES EL ENUM
            })
            .ToListAsync();

        return Ok(partidos);
    }

    [HttpGet("mispartidos")]
    public async Task<ActionResult<List<PartidoDto>>> GetMisPartidos()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var misPartidos = await _context.Partidos
            .Where(p => (p.EquipoA != null && p.EquipoA.Miembros.Any(m => m.PerfilJuego.UsuarioId == userId)) ||
                        (p.EquipoB != null && p.EquipoB.Miembros.Any(m => m.PerfilJuego.UsuarioId == userId)))
            .Select(p => new PartidoDto
            {
                Id = p.Id,
                Ronda = p.Ronda,
                IndicePartido = p.IndicePartido,
                EquipoAId = p.EquipoA_Id,
                EquipoANombre = p.EquipoA != null ? p.EquipoA.Nombre : "TBD",
                EquipoBId = p.EquipoB_Id,
                EquipoBNombre = p.EquipoB != null ? p.EquipoB.Nombre : "TBD",
                EquipoGanadorId = p.EquipoGanadorId,
                Estado = p.Estado // <-- YA ES EL ENUM
            })
            .ToListAsync();

        return Ok(misPartidos);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<PartidoDetalleDto>> GetPartido(int id)
    {
        var partidoDto = await _context.Partidos
            .Where(p => p.Id == id)
            .Select(p => new PartidoDetalleDto
            {
                Id = p.Id,
                Ronda = p.Ronda,
                IndicePartido = p.IndicePartido,
                EquipoAId = p.EquipoA_Id,
                EquipoANombre = p.EquipoA != null ? p.EquipoA.Nombre : "TBD",
                EquipoBId = p.EquipoB_Id,
                EquipoBNombre = p.EquipoB != null ? p.EquipoB.Nombre : "TBD",
                EquipoGanadorId = p.EquipoGanadorId,
                Estado = p.Estado, // <-- YA ES EL ENUM
                TorneoTitulo = p.Torneo.Titulo
            })
            .FirstOrDefaultAsync();

        if (partidoDto == null) return NotFound("Partido no encontrado.");

        return Ok(partidoDto);
    }

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
            partido.Estado = DEFIANTS.Shared.Enums.EstadoPartido.Finalizado;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }
}

using System.Security.Claims;
using DEFIANTS.Server.Data;
using DEFIANTS.Server.Models.Entities;
using DEFIANTS.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DEFIANTS.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PerfilesJuegoController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PerfilesJuegoController(ApplicationDbContext context)
    {
        _context = context;
    }

    // --- MÉTODO MODIFICADO PARA USAR DTOs ---
    [HttpGet("misperfiles")]
    public async Task<ActionResult<List<PerfilJuegoDto>>> GetMisPerfiles()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var perfiles = await _context.PerfilesJuego
            .Where(p => p.UsuarioId == userId)
            .Include(p => p.Juego)
            .Select(p => new PerfilJuegoDto
            {
                Id = p.Id,
                JuegoId = p.JuegoId,
                JuegoNombre = p.Juego.Nombre,
                NicknameInGame = p.NicknameInGame,
                Elo = p.Elo
            })
            .ToListAsync();

        return Ok(perfiles);
    }

    // ... (resto de los métodos) ...
    [HttpPost]
    public async Task<IActionResult> CrearPerfilJuego([FromBody] CrearPerfilJuegoDto perfilDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var perfilExistente = await _context.PerfilesJuego
            .AnyAsync(p => p.UsuarioId == userId && p.JuegoId == perfilDto.JuegoId);

        if (perfilExistente)
        {
            return Conflict("Ya tienes un perfil para este juego.");
        }

        var nuevoPerfil = new PerfilJuego
        {
            UsuarioId = userId,
            JuegoId = perfilDto.JuegoId,
            NicknameInGame = perfilDto.NicknameInGame,
            Elo = 0
        };

        _context.PerfilesJuego.Add(nuevoPerfil);
        await _context.SaveChangesAsync();

        return Ok(nuevoPerfil);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> ActualizarPerfilJuego(int id, [FromBody] ActualizarPerfilJuegoDto perfilDto)
    {
        var perfil = await _context.PerfilesJuego.FindAsync(id);
        if (perfil == null) return NotFound("Perfil de juego no encontrado.");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (perfil.UsuarioId != userId)
        {
            return Forbid("No tienes permiso para editar este perfil de juego.");
        }

        perfil.NicknameInGame = perfilDto.NicknameInGame;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

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

    // GET: api/perfilesjuego/misperfiles
    [HttpGet("misperfiles")]
    public async Task<IActionResult> GetMisPerfiles()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var perfiles = await _context.PerfilesJuego
            .Where(p => p.UsuarioId == userId)
            .Include(p => p.Juego)
            .Select(p => new 
            {
                p.Id,
                p.JuegoId,
                JuegoNombre = p.Juego.Nombre,
                p.NicknameInGame,
                p.Elo
            })
            .ToListAsync();

        return Ok(perfiles);
    }

    // POST: api/perfilesjuego
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

    // PUT: api/perfilesjuego/5
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

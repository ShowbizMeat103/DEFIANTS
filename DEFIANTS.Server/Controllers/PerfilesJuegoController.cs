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
                JuegoLogoUrl = p.Juego.LogoUrl,
                JuegoTieneSistemaElo = p.Juego.TieneSistemaElo,
                NicknameInGame = p.NicknameInGame,
                Elo = p.Elo
            })
            .ToListAsync();

        return Ok(perfiles);
    }

    [HttpPost]
    public async Task<ActionResult<PerfilJuegoDto>> CrearPerfilJuego([FromBody] CrearPerfilJuegoDto perfilDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var juego = await _context.Juegos.FindAsync(perfilDto.JuegoId);
        if (juego == null)
        {
            return BadRequest("El juego especificado no existe.");
        }

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
            Elo = juego.TieneSistemaElo ? perfilDto.Elo : 0
        };

        _context.PerfilesJuego.Add(nuevoPerfil);
        await _context.SaveChangesAsync();

        var responseDto = new PerfilJuegoDto
        {
            Id = nuevoPerfil.Id,
            JuegoId = nuevoPerfil.JuegoId,
            JuegoNombre = juego.Nombre,
            JuegoLogoUrl = juego.LogoUrl,
            JuegoTieneSistemaElo = juego.TieneSistemaElo,
            NicknameInGame = nuevoPerfil.NicknameInGame,
            Elo = nuevoPerfil.Elo
        };

        return Ok(responseDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> ActualizarPerfilJuego(int id, [FromBody] ActualizarPerfilJuegoDto perfilDto)
    {
        var perfil = await _context.PerfilesJuego
            .Include(p => p.Juego)
            .FirstOrDefaultAsync(p => p.Id == id);
            
        if (perfil == null) return NotFound("Perfil de juego no encontrado.");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (perfil.UsuarioId != userId)
        {
            return Forbid("No tienes permiso para editar este perfil de juego.");
        }

        perfil.NicknameInGame = perfilDto.NicknameInGame;
        
        if (perfil.Juego.TieneSistemaElo && perfilDto.Elo.HasValue)
        {
            perfil.Elo = perfilDto.Elo.Value;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }
    
    [HttpDelete("eliminar/{id}")]
    public async Task<IActionResult> EliminarPerfilJuego(int id)
    {
        var perfil = await _context.PerfilesJuego.FindAsync(id);
        if (perfil == null)
        {
            return NotFound("Perfil de juego no encontrado.");
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (perfil.UsuarioId != userId)
        {
            return Forbid("No tienes permiso para eliminar este perfil.");
        }

        _context.PerfilesJuego.Remove(perfil);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

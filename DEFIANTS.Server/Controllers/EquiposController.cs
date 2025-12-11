using System.Security.Claims;
using DEFIANTS.Server.Data;
using DEFIANTS.Server.Models.Entities;
using DEFIANTS.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DEFIANTS.Shared.Enums;
using System.Linq;

namespace DEFIANTS.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EquiposController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<Usuario> _userManager;

    public EquiposController(ApplicationDbContext context, UserManager<Usuario> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<EquipoResumenDto>>> GetEquipos()
    {
        var equipos = await _context.Equipos
            .Select(e => new EquipoResumenDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                JuegoId = e.JuegoId 
            })
            .ToListAsync();
            
        return Ok(equipos);
    }

    [HttpGet("misequipos")]
    public async Task<ActionResult<List<MiEquipoDto>>> GetMisEquipos()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var misEquipos = await _context.MiembrosEquipo
            .Where(m => m.PerfilJuego.UsuarioId == userId)
            .Select(m => new MiEquipoDto
            {
                Id = m.Equipo.Id,
                Nombre = m.Equipo.Nombre,
                JuegoId = m.Equipo.JuegoId,
                Rol = m.Rol,
                CantidadMiembros = m.Equipo.Miembros.Count() 
            })
            .ToListAsync();

        return Ok(misEquipos);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<EquipoDetalleDto>> GetEquipo(int id)
    {
        var equipoDto = await _context.Equipos
            .Where(e => e.Id == id)
            .Select(e => new EquipoDetalleDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                JuegoId = e.JuegoId,
                CapitanId = e.CapitanId,
                Miembros = e.Miembros.Select(m => new MiembroEquipoDto
                {
                    Id = m.Id,
                    NicknameInGame = m.PerfilJuego.NicknameInGame,
                    Rol = m.Rol
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (equipoDto == null)
        {
            return NotFound();
        }

        return Ok(equipoDto);
    }

    [HttpPost]
    public async Task<IActionResult> CrearEquipo([FromBody] CrearEquipoDto equipoDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized("No se pudo identificar al usuario.");

        var perfilJuego = await _context.PerfilesJuego
            .FirstOrDefaultAsync(p => p.UsuarioId == userId && p.JuegoId == equipoDto.JuegoId);

        if (perfilJuego == null) return BadRequest($"No tienes un perfil de juego para el juego seleccionado. Por favor, crea uno primero.");

        var nuevoEquipo = new Equipo
        {
            Nombre = equipoDto.Nombre,
            JuegoId = equipoDto.JuegoId,
            CapitanId = perfilJuego.Id
        };

        _context.Equipos.Add(nuevoEquipo);
        await _context.SaveChangesAsync();

        var miembroCapitan = new MiembroEquipo
        {
            EquipoId = nuevoEquipo.Id,
            PerfilJuegoId = perfilJuego.Id,
            Rol = RolEquipo.Lider
        };
        _context.MiembrosEquipo.Add(miembroCapitan);
        await _context.SaveChangesAsync();

        var equipoCreadoDto = new EquipoDetalleDto
        {
            Id = nuevoEquipo.Id,
            Nombre = nuevoEquipo.Nombre,
            JuegoId = nuevoEquipo.JuegoId,
            CapitanId = nuevoEquipo.CapitanId,
            Miembros = new List<MiembroEquipoDto>
            {
                new MiembroEquipoDto
                {
                    Id = miembroCapitan.Id,
                    NicknameInGame = perfilJuego.NicknameInGame,
                    Rol = miembroCapitan.Rol
                }
            }
        };
        return StatusCode(StatusCodes.Status201Created, equipoCreadoDto);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> ActualizarEquipo(int id, [FromBody] CrearEquipoDto equipoDto)
    {
        var equipo = await _context.Equipos.FindAsync(id);
        if (equipo == null) return NotFound("Equipo no encontrado.");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var perfilJuegoCapitan = await _context.PerfilesJuego.FirstOrDefaultAsync(p => p.UsuarioId == userId && p.JuegoId == equipo.JuegoId);

        if (perfilJuegoCapitan == null || equipo.CapitanId != perfilJuegoCapitan.Id)
        {
            return Forbid("Solo el capitán del equipo puede editarlo.");
        }

        equipo.Nombre = equipoDto.Nombre;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{equipoId}/miembros/{miembroId}/rol")]
    public async Task<IActionResult> ActualizarRolMiembro(int equipoId, int miembroId, [FromBody] ActualizarRolMiembroDto rolDto)
    {
        var equipo = await _context.Equipos.Include(e => e.Miembros).FirstOrDefaultAsync(e => e.Id == equipoId);
        if (equipo == null) return NotFound("Equipo no encontrado.");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var perfilJuegoCapitan = await _context.PerfilesJuego.FirstOrDefaultAsync(p => p.UsuarioId == userId && p.JuegoId == equipo.JuegoId);

        if (perfilJuegoCapitan == null || equipo.CapitanId != perfilJuegoCapitan.Id)
        {
            return Forbid("Solo el capitán del equipo puede cambiar los roles de los miembros.");
        }

        var miembro = equipo.Miembros.FirstOrDefault(m => m.Id == miembroId);
        if (miembro == null) return NotFound("Miembro no encontrado en este equipo.");

        if (miembro.PerfilJuegoId == equipo.CapitanId && rolDto.Rol != RolEquipo.Lider)
        {
            return BadRequest("No puedes cambiar el rol del capitán a algo que no sea 'Lider'. Transfiere la capitanía primero.");
        }
        
        miembro.Rol = rolDto.Rol;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{equipoId}/miembros/{miembroId}")]
    public async Task<IActionResult> ExpulsarMiembro(int equipoId, int miembroId)
    {
        var equipo = await _context.Equipos.Include(e => e.Miembros).FirstOrDefaultAsync(e => e.Id == equipoId);
        if (equipo == null) return NotFound("Equipo no encontrado.");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var perfilJuegoCapitan = await _context.PerfilesJuego.FirstOrDefaultAsync(p => p.UsuarioId == userId && p.JuegoId == equipo.JuegoId);

        if (perfilJuegoCapitan == null || equipo.CapitanId != perfilJuegoCapitan.Id)
        {
            return Forbid("Solo el capitán del equipo puede expulsar miembros.");
        }

        var miembroAExpulsar = equipo.Miembros.FirstOrDefault(m => m.Id == miembroId);
        if (miembroAExpulsar == null) return NotFound("Miembro no encontrado en este equipo.");

        if (miembroAExpulsar.PerfilJuegoId == equipo.CapitanId)
        {
            return BadRequest("No puedes expulsar al capitán del equipo.");
        }

        _context.MiembrosEquipo.Remove(miembroAExpulsar);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DisolverEquipo(int id)
    {
        var equipo = await _context.Equipos.FindAsync(id);
        if (equipo == null) return NotFound("Equipo no encontrado.");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var perfilJuegoCapitan = await _context.PerfilesJuego.FirstOrDefaultAsync(p => p.UsuarioId == userId && p.JuegoId == equipo.JuegoId);

        if (perfilJuegoCapitan == null || equipo.CapitanId != perfilJuegoCapitan.Id)
        {
            return Forbid("Solo el capitán del equipo puede disolverlo.");
        }
        
        _context.MiembrosEquipo.RemoveRange(_context.MiembrosEquipo.Where(m => m.EquipoId == id));
        _context.Equipos.Remove(equipo);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{equipoId}/miembros")]
    public async Task<IActionResult> AnadirMiembro(int equipoId, [FromBody] InvitarMiembroDto miembroDto)
    {
        var equipo = await _context.Equipos.Include(e => e.Miembros).FirstOrDefaultAsync(e => e.Id == equipoId);
        if (equipo == null) return NotFound("El equipo no existe.");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var perfilJuegoCapitan = await _context.PerfilesJuego.FirstOrDefaultAsync(p => p.UsuarioId == userId && p.JuegoId == equipo.JuegoId);

        if (perfilJuegoCapitan == null || equipo.CapitanId != perfilJuegoCapitan.Id)
        {
            return Forbid("Solo el capitán del equipo puede añadir nuevos miembros.");
        }

        var usuarioAInvitar = await _userManager.FindByNameAsync(miembroDto.Username);
        if (usuarioAInvitar == null) return BadRequest("El usuario a invitar no existe.");

        var perfilAInvitar = await _context.PerfilesJuego.FirstOrDefaultAsync(p => p.UsuarioId == usuarioAInvitar.Id && p.JuegoId == equipo.JuegoId);
        if (perfilAInvitar == null) return BadRequest($"El usuario '{miembroDto.Username}' no tiene un perfil para este juego.");

        if (equipo.Miembros.Any(m => m.PerfilJuegoId == perfilAInvitar.Id))
        {
            return Conflict("Este jugador ya es miembro del equipo.");
        }

        var nuevoMiembro = new MiembroEquipo
        {
            EquipoId = equipoId,
            PerfilJuegoId = perfilAInvitar.Id,
            Rol = RolEquipo.Titular
        };
        _context.MiembrosEquipo.Add(nuevoMiembro);
        await _context.SaveChangesAsync();

        return Ok(new { message = $"'{miembroDto.Username}' ha sido añadido al equipo." });
    }
}

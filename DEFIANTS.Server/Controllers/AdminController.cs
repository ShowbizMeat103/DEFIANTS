using DEFIANTS.Server.Data;
using DEFIANTS.Server.Models.Entities;
using DEFIANTS.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DEFIANTS.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly UserManager<Usuario> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;

    public AdminController(UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    // ... (otros métodos sin cambios) ...
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userManager.Users
            .Select(u => new { u.Id, u.UserName, u.Email })
            .ToListAsync();
        return Ok(users);
    }
    
    [HttpGet("users/{id}")]
    public async Task<IActionResult> GetUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound("Usuario no encontrado.");

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new
        {
            user.Id,
            user.UserName,
            user.Email,
            Roles = roles
        });
    }

    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] UpdateRoleDto updateRoleDto)
    {
        var user = await _userManager.FindByNameAsync(updateRoleDto.Username);
        if (user == null) return BadRequest("Usuario no encontrado.");
        var roleExists = await _roleManager.RoleExistsAsync(updateRoleDto.RoleName);
        if (!roleExists) return BadRequest("Rol no encontrado.");
        var result = await _userManager.AddToRoleAsync(user, updateRoleDto.RoleName);
        if (result.Succeeded) return Ok(new { message = $"Rol '{updateRoleDto.RoleName}' asignado a '{updateRoleDto.Username}' correctamente." });
        return BadRequest(result.Errors);
    }
    
    [HttpDelete("users/{id}/roles/{roleName}")]
    public async Task<IActionResult> RemoveRoleFromUser(string id, string roleName)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound("Usuario no encontrado.");

        var result = await _userManager.RemoveFromRoleAsync(user, roleName);
        if (result.Succeeded) return NoContent();

        return BadRequest(result.Errors);
    }

    [HttpPost("juegos")]
    public async Task<IActionResult> CrearJuego([FromBody] CrearJuegoDto juegoDto)
    {
        var nuevoJuego = new Juego
        {
            Nombre = juegoDto.Nombre,
            LogoUrl = juegoDto.LogoUrl,
            IntegrantesPorEquipo = juegoDto.IntegrantesPorEquipo,
            TieneSistemaElo = juegoDto.TieneSistemaElo
        };

        _context.Juegos.Add(nuevoJuego);
        await _context.SaveChangesAsync();

        // --- SOLUCIÓN: Devolver un 201 Created con el objeto directamente ---
        return StatusCode(StatusCodes.Status201Created, nuevoJuego);
    }

    [HttpPut("juegos/{id}")]
    public async Task<IActionResult> ActualizarJuego(int id, [FromBody] CrearJuegoDto juegoDto)
    {
        var juego = await _context.Juegos.FindAsync(id);
        if (juego == null) return NotFound("Juego no encontrado.");
        juego.Nombre = juegoDto.Nombre;
        juego.LogoUrl = juegoDto.LogoUrl;
        juego.IntegrantesPorEquipo = juegoDto.IntegrantesPorEquipo;
        juego.TieneSistemaElo = juegoDto.TieneSistemaElo;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

using DEFIANTS.Server.Data;
using DEFIANTS.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DEFIANTS.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JuegosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public JuegosController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<JuegoDto>>> GetJuegos()
    {
        var juegos = await _context.Juegos
            .Select(j => new JuegoDto
            {
                Id = j.Id,
                Nombre = j.Nombre,
                LogoUrl = j.LogoUrl,
                IntegrantesPorEquipo = j.IntegrantesPorEquipo,
                TieneSistemaElo = j.TieneSistemaElo
            })
            .ToListAsync();
        return Ok(juegos);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<JuegoDto>> GetJuego(int id)
    {
        var juego = await _context.Juegos
            .Select(j => new JuegoDto
            {
                Id = j.Id,
                Nombre = j.Nombre,
                LogoUrl = j.LogoUrl,
                IntegrantesPorEquipo = j.IntegrantesPorEquipo,
                TieneSistemaElo = j.TieneSistemaElo
            })
            .FirstOrDefaultAsync(j => j.Id == id);

        if (juego == null)
        {
            return NotFound();
        }
        return Ok(juego);
    }
}

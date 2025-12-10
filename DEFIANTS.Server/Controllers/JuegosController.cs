using DEFIANTS.Server.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    // GET: api/juegos
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetJuegos()
    {
        var juegos = await _context.Juegos.ToListAsync();
        return Ok(juegos);
    }

    // GET: api/juegos/5
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetJuego(int id)
    {
        var juego = await _context.Juegos.FindAsync(id);
        if (juego == null)
        {
            return NotFound();
        }
        return Ok(juego);
    }
}

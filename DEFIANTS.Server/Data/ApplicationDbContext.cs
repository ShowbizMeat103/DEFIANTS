using DEFIANTS.Server.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DEFIANTS.Server.Data;

// Heredamos de IdentityDbContext en lugar de DbContext.
// Le pasamos nuestra clase de Usuario personalizada.
public class ApplicationDbContext : IdentityDbContext<Usuario>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // Ya no necesitamos el DbSet<Usuario> porque IdentityDbContext lo gestiona.
    // public DbSet<Usuario> Usuarios { get; set; } 
    
    public DbSet<Juego> Juegos { get; set; }
    public DbSet<PerfilJuego> PerfilesJuego { get; set; }
    public DbSet<Equipo> Equipos { get; set; }
    public DbSet<MiembroEquipo> MiembrosEquipo { get; set; }
    public DbSet<Torneo> Torneos { get; set; }
    public DbSet<Inscripcion> Inscripciones { get; set; }
    public DbSet<Partido> Partidos { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Aquí puedes añadir configuraciones adicionales del modelo si es necesario.
    }
}

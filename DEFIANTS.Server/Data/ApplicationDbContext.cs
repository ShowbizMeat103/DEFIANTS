using DEFIANTS.Server.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DEFIANTS.Server.Data;

public class ApplicationDbContext : IdentityDbContext<Usuario>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

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
    }
}

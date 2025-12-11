using System.ComponentModel.DataAnnotations;
using DEFIANTS.Shared.Enums;


namespace DEFIANTS.Server.Models.Entities;

public class Torneo
{
    [Key] public int Id { get; set; }
    public string Titulo { get; set; }
    public int MaxEquipos { get; set; }
    public EstadoTorneo Status { get; set; }
    public int JuegoId { get; set; }
    public virtual Juego Juego { get; set; }
    
    public decimal PrecioInscripcion { get; set; }
    public decimal PrizePool { get; set; }
    public DateTime FechaInicio { get; set; }
    
    [Required]
    public string CreadorId { get; set; }
    public virtual Usuario Creador { get; set; }

    public virtual ICollection<Inscripcion> Inscripciones { get; set; }
    public virtual ICollection<Partido> Partidos { get; set; }
}

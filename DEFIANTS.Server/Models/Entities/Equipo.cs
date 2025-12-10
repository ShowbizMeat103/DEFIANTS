using System.ComponentModel.DataAnnotations;

namespace DEFIANTS.Server.Models.Entities;

public class Equipo
{
    [Key] public int Id { get; set; }
    public string Nombre { get; set; }
    
    // Un equipo es de un juego, por tanto el capitán es un perfil de ese juego.
    public int CapitanId { get; set; } // FK a PerfilJuego

    public int JuegoId { get; set; }
    public virtual ICollection<MiembroEquipo> Miembros { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace DEFIANTS.Server.Models.Entities;

public class Equipo
{
    [Key] public int Id { get; set; }
    public string Nombre { get; set; }
    
    public int CapitanId { get; set; }

    public int JuegoId { get; set; }
    public virtual ICollection<MiembroEquipo> Miembros { get; set; }
}

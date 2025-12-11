using System.ComponentModel.DataAnnotations;
using DEFIANTS.Shared.Enums;

namespace DEFIANTS.Server.Models.Entities;

public class MiembroEquipo
{
    [Key] public int Id { get; set; }
    public int EquipoId { get; set; }
    public int PerfilJuegoId { get; set; }
    public RolEquipo Rol { get; set; }

    public virtual PerfilJuego PerfilJuego { get; set; }

    public virtual Equipo Equipo { get; set; }
}

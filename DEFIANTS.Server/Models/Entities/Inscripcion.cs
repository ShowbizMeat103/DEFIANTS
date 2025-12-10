using System.ComponentModel.DataAnnotations;
using DEFIANTS.Shared.Enums; // <-- NAMESPACE ACTUALIZADO

namespace DEFIANTS.Server.Models.Entities;

public class Inscripcion
{
    [Key] public int Id { get; set; }
    public int TorneoId { get; set; }
    public int EquipoId { get; set; }
    public virtual Equipo Equipo { get; set; }
    public EstadoPago EstadoPago { get; set; }
    public virtual Torneo Torneo { get; set; }
}

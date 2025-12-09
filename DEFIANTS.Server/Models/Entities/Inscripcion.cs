using System.ComponentModel.DataAnnotations;
using DEFIANTS.Server.Models.Enums;

namespace DEFIANTS.Server.Models.Entities;

public class Inscripcion
{
    [Key] public int Id { get; set; }
    public int TorneoId { get; set; }
    public int EquipoId { get; set; }
    public virtual Equipo Equipo { get; set; }
    public EstadoPago EstadoPago { get; set; }

    // --- PROPIEDAD DE NAVEGACIÓN AÑADIDA ---
    public virtual Torneo Torneo { get; set; }
    // -----------------------------------------
}

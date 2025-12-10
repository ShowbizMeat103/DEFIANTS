using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DEFIANTS.Server.Models.Enums;

namespace DEFIANTS.Server.Models.Entities;

public class Partido
{
    [Key] public int Id { get; set; }
    public int TorneoId { get; set; }

    // --- PROPIEDAD DE NAVEGACIÓN AÑADIDA ---
    public virtual Torneo Torneo { get; set; }
    // -----------------------------------------

    public int Ronda { get; set; }
    public int IndicePartido { get; set; }

    public int? PartidoSiguienteId { get; set; }
    [ForeignKey("PartidoSiguienteId")]
    public virtual Partido? PartidoSiguiente { get; set; }

    public int? EquipoA_Id { get; set; }
    [ForeignKey("EquipoA_Id")]
    public virtual Equipo? EquipoA { get; set; }

    public int? EquipoB_Id { get; set; }
    [ForeignKey("EquipoB_Id")]
    public virtual Equipo? EquipoB { get; set; }

    public int? EquipoGanadorId { get; set; }
    public int ScoreA { get; set; }
    public int ScoreB { get; set; }
    public EstadoPartido Estado { get; set; }
}

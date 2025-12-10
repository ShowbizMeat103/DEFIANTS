using DEFIANTS.Shared.Enums; // <-- AÑADIDO

namespace DEFIANTS.Shared.DTOs;

public class PartidoDto
{
    public int Id { get; set; }
    public int Ronda { get; set; }
    public int IndicePartido { get; set; }
    public int? EquipoAId { get; set; }
    public string? EquipoANombre { get; set; }
    public int? EquipoBId { get; set; }
    public string? EquipoBNombre { get; set; }
    public int? EquipoGanadorId { get; set; }
    public EstadoPartido Estado { get; set; } // <-- CAMBIADO DE string A Enum
}

using DEFIANTS.Shared.Enums;

namespace DEFIANTS.Shared.DTOs;

public class InscripcionDetalleDto
{
    public int Id { get; set; }
    public int EquipoId { get; set; }
    public string NombreEquipo { get; set; }
    public EstadoPago EstadoPago { get; set; }
}

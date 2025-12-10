using DEFIANTS.Shared.Enums;

namespace DEFIANTS.Shared.DTOs;

public class MiEquipoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public int JuegoId { get; set; }
    public RolEquipo Rol { get; set; }
    public int CantidadMiembros { get; set; } // <-- AÑADIDO
}

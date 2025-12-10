using DEFIANTS.Shared.Enums;

namespace DEFIANTS.Shared.DTOs;

public class MiembroEquipoDto
{
    public int Id { get; set; }
    public string NicknameInGame { get; set; }
    public RolEquipo Rol { get; set; }
}

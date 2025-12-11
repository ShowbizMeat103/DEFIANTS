using System.ComponentModel.DataAnnotations;
using DEFIANTS.Shared.Enums;

namespace DEFIANTS.Shared.DTOs;

public class ActualizarRolMiembroDto
{
    [Required]
    public RolEquipo Rol { get; set; }
}

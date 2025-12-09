using System.ComponentModel.DataAnnotations;
using DEFIANTS.Shared.Enums; // Asumiendo que los enums se moverán a Shared

namespace DEFIANTS.Shared.DTOs;

public class ActualizarRolMiembroDto
{
    [Required]
    public RolEquipo Rol { get; set; }
}

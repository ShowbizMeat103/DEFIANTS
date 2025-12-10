using System.ComponentModel.DataAnnotations;

namespace DEFIANTS.Shared.DTOs;

public class UpdateRoleDto
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string RoleName { get; set; }
}

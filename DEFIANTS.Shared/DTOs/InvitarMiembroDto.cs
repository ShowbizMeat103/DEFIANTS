using System.ComponentModel.DataAnnotations;

namespace DEFIANTS.Shared.DTOs;

public class InvitarMiembroDto
{
    [Required(ErrorMessage = "El nombre de usuario del jugador a invitar es obligatorio.")]
    public string Username { get; set; }
}

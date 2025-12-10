using System.ComponentModel.DataAnnotations;

namespace DEFIANTS.Shared.DTOs;

public class RegisterDto
{
    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    public string Username { get; set; }

    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    public string Password { get; set; }
}

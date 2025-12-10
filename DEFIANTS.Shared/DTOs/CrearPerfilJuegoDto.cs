using System.ComponentModel.DataAnnotations;

namespace DEFIANTS.Shared.DTOs;

public class CrearPerfilJuegoDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar un juego.")]
    public int JuegoId { get; set; }

    [Required(ErrorMessage = "El nombre en el juego es obligatorio.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres.")]
    public string NicknameInGame { get; set; } = string.Empty;
}

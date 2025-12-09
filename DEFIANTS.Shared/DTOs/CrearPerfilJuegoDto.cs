using System.ComponentModel.DataAnnotations;

namespace DEFIANTS.Shared.DTOs;

public class CrearPerfilJuegoDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar un juego válido.")]
    public int JuegoId { get; set; }

    [Required(ErrorMessage = "El nickname en el juego es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nickname no puede tener más de 100 caracteres.")]
    public string NicknameInGame { get; set; }
}

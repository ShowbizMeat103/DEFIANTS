using System.ComponentModel.DataAnnotations;

namespace DEFIANTS.Shared.DTOs;

public class CrearPerfilJuegoDto
{
    [Required(ErrorMessage = "El ID del juego es obligatorio.")]
    public int JuegoId { get; set; }

    [Required(ErrorMessage = "El nickname en el juego es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nickname no puede tener más de 100 caracteres.")]
    public string NicknameInGame { get; set; }

    [Range(0, 3000, ErrorMessage = "El ELO debe estar entre 0 y 3000.")] // Rango de ejemplo, ajusta según necesites
    public int Elo { get; set; } = 0; // Valor por defecto
}

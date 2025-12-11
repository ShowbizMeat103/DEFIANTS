using System.ComponentModel.DataAnnotations;

namespace DEFIANTS.Shared.DTOs;

public class ActualizarPerfilJuegoDto
{
    [Required(ErrorMessage = "El nickname en el juego es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nickname no puede tener más de 100 caracteres.")]
    public string NicknameInGame { get; set; }

    [Range(0, 3000, ErrorMessage = "El ELO debe estar entre 0 y 3000.")]
    public int? Elo { get; set; } // <-- CAMBIADO A NULLABLE
}

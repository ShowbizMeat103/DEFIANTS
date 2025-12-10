using System.ComponentModel.DataAnnotations;

namespace DEFIANTS.Shared.DTOs;

public class ActualizarPerfilJuegoDto
{
    public string NicknameInGame { get; set; } = string.Empty;

    [Range(0, 5000, ErrorMessage = "El ELO debe ser un valor válido.")]
    public int? Elo { get; set; } // Hacemos que sea nullable para que no sea obligatorio si solo se actualiza el nickname
}

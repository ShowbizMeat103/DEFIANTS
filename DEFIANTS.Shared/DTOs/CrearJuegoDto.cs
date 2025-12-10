using System.ComponentModel.DataAnnotations;

namespace DEFIANTS.Shared.DTOs;

public class CrearJuegoDto
{
    [Required(ErrorMessage = "El nombre del juego es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Url(ErrorMessage = "Debe ser una URL válida.")]
    public string? LogoUrl { get; set; }

    [Range(1, 10, ErrorMessage = "El número de integrantes debe ser entre 1 y 10.")]
    public int IntegrantesPorEquipo { get; set; }

    public bool TieneSistemaElo { get; set; }
}

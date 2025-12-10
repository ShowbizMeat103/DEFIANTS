using System.ComponentModel.DataAnnotations;

namespace DEFIANTS.Shared.DTOs;

public class CrearJuegoDto
{
    [Required(ErrorMessage = "El nombre del juego es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres.")]
    public string Nombre { get; set; }

    public string? LogoUrl { get; set; } // Opcional

    [Range(1, 10, ErrorMessage = "El número de integrantes por equipo debe ser entre 1 y 10.")]
    public int IntegrantesPorEquipo { get; set; } = 5; // Valor por defecto

    public bool TieneSistemaElo { get; set; } = false; // Valor por defecto
}

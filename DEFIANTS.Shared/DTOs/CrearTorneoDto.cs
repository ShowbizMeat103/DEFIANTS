using System.ComponentModel.DataAnnotations;

namespace DEFIANTS.Shared.DTOs;

public class CrearTorneoDto
{
    [Required(ErrorMessage = "El título del torneo es obligatorio.")]
    [StringLength(100, ErrorMessage = "El título no puede tener más de 100 caracteres.")]
    public string Titulo { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar un juego válido.")]
    public int JuegoId { get; set; }

    [Range(4, 128, ErrorMessage = "El número de equipos debe estar entre 4 y 128.")]
    public int MaxEquipos { get; set; }

    [Range(0, 100000, ErrorMessage = "El precio de inscripción no es válido.")]
    public decimal PrecioInscripcion { get; set; }

    [Range(0, 1000000, ErrorMessage = "El pozo de premios no es válido.")]
    public decimal PrizePool { get; set; }

    public DateTime FechaInicio { get; set; }
}

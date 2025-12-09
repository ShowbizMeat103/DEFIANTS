using System.ComponentModel.DataAnnotations;

namespace DEFIANTS.Shared.DTOs;

public class CrearEquipoDto
{
    [Required(ErrorMessage = "El nombre del equipo es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres.")]
    public string Nombre { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar un juego válido.")]
    public int JuegoId { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace DEFIANTS.Shared.DTOs;

public class InscribirEquipoDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Se debe proporcionar un ID de equipo válido.")]
    public int EquipoId { get; set; }
}

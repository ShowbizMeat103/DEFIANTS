using System.ComponentModel.DataAnnotations;

namespace DEFIANTS.Shared.DTOs;

public class CorregirPartidoDto
{
    [Range(0, 100)]
    public int ScoreA { get; set; }

    [Range(0, 100)]
    public int ScoreB { get; set; }

    public int? EquipoGanadorId { get; set; }
}

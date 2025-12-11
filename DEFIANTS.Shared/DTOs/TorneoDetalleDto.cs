using System;
using System.Collections.Generic;
using DEFIANTS.Shared.Enums;

namespace DEFIANTS.Shared.DTOs;

public class TorneoDetalleDto
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string Status { get; set; }
    public int JuegoId { get; set; }
    public string JuegoNombre { get; set; }
    public int MaxEquipos { get; set; }
    public decimal PrecioInscripcion { get; set; }
    public decimal PrizePool { get; set; }
    public DateTime FechaInicio { get; set; }
    public string CreadorId { get; set; }
    public List<PartidoDto> Partidos { get; set; } = new();
    public List<InscripcionDetalleDto> Inscripciones { get; set; } = new();
}

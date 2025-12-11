using System;
using DEFIANTS.Shared.Enums;

namespace DEFIANTS.Shared.DTOs;

public class TorneoDto
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public int JuegoId { get; set; }
    public int MaxEquipos { get; set; }
    public decimal PrecioInscripcion { get; set; }
    public decimal PrizePool { get; set; }
    public DateTime FechaInicio { get; set; }
    public EstadoTorneo Status { get; set; }
    public string CreadorId { get; set; }
}

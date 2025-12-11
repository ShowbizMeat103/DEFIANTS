using System;

namespace DEFIANTS.Shared.DTOs;

public class TorneoResumenDto
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string Status { get; set; }
    public int MaxEquipos { get; set; }
    public DateTime FechaInicio { get; set; } // <-- AÑADIDO
}

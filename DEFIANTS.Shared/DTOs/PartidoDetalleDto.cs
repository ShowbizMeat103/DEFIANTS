using DEFIANTS.Shared.Enums; // <-- AÑADIDO

namespace DEFIANTS.Shared.DTOs;

public class PartidoDetalleDto : PartidoDto
{
    public string? TorneoTitulo { get; set; }
    // EquipoA_Nombre y EquipoB_Nombre ya están en PartidoDto
}

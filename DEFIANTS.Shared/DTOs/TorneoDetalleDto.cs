namespace DEFIANTS.Shared.DTOs;

public class TorneoDetalleDto
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string Status { get; set; }
    public List<PartidoDto> Partidos { get; set; } = new();
}

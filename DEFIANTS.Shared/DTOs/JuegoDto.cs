namespace DEFIANTS.Shared.DTOs;

public class JuegoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string? LogoUrl { get; set; }
    public int IntegrantesPorEquipo { get; set; }
    public bool TieneSistemaElo { get; set; }
}

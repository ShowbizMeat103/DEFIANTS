namespace DEFIANTS.Shared.DTOs;

public class PerfilJuegoDto
{
    public int Id { get; set; }
    public int JuegoId { get; set; }
    public string JuegoNombre { get; set; } = string.Empty;
    public string NicknameInGame { get; set; } = string.Empty;
    public int Elo { get; set; }
    
    // --- PROPIEDADES RESTAURADAS ---
    public string? JuegoLogoUrl { get; set; }
    public bool JuegoTieneSistemaElo { get; set; }
}

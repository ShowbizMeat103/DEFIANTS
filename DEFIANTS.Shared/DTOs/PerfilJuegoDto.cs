namespace DEFIANTS.Shared.DTOs;

public class PerfilJuegoDto
{
    public int Id { get; set; }
    public int JuegoId { get; set; }
    public string JuegoNombre { get; set; }
    public string NicknameInGame { get; set; }
    public int Elo { get; set; } // <-- AÑADIDO
}

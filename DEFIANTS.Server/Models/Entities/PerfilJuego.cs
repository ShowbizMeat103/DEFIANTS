using System.ComponentModel.DataAnnotations;

namespace DEFIANTS.Server.Models.Entities;

public class PerfilJuego
{
    [Key] public int Id { get; set; }
    
    public string UsuarioId { get; set; } 
    public virtual Usuario Usuario { get; set; }
    
    public int JuegoId { get; set; }
    public virtual Juego Juego { get; set; }

    public string NicknameInGame { get; set; }
    public int Elo { get; set; }
}

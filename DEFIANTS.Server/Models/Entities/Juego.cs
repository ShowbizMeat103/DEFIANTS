using System.ComponentModel.DataAnnotations;

namespace DEFIANTS.Server.Models.Entities;

public class Juego
{
    [Key] public int Id { get; set; }
    [Required] public string Nombre { get; set; }
    
    // --- CORRECCIÓN AQUÍ ---
    public string? LogoUrl { get; set; } // Se permite que sea nulo
    // -------------------------

    public int IntegrantesPorEquipo { get; set; }
    public bool TieneSistemaElo { get; set; }
}

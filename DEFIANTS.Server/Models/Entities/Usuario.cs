using Microsoft.AspNetCore.Identity;

namespace DEFIANTS.Server.Models.Entities;

// Heredamos de IdentityUser para obtener toda la funcionalidad de autenticación.
// El tipo de la clave primaria por defecto es string.
public class Usuario : IdentityUser
{
    // Aquí puedes añadir propiedades adicionales que no estén en IdentityUser.
    // Por ejemplo:
    // public string? NombreReal { get; set; }
    // public DateTime FechaNacimiento { get; set; }

    // La relación con PerfilJuego se mantiene.
    public virtual ICollection<PerfilJuego> PerfilesJuego { get; set; } = new List<PerfilJuego>();
}

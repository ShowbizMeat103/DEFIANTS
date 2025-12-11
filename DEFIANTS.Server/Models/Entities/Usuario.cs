using Microsoft.AspNetCore.Identity;

namespace DEFIANTS.Server.Models.Entities;

public class Usuario : IdentityUser
{
    public virtual ICollection<PerfilJuego> PerfilesJuego { get; set; } = new List<PerfilJuego>();
}

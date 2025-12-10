using System.Collections.Generic;

namespace DEFIANTS.Shared.DTOs;

public class UsuarioDto
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public List<string> Roles { get; set; } = new();
}

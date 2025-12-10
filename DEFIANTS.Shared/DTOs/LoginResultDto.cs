using System;

namespace DEFIANTS.Shared.DTOs;

public class LoginResultDto
{
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
}

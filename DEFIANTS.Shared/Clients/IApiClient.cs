using DEFIANTS.Shared.DTOs;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEFIANTS.Shared.Clients;

public interface IApiClient
{
    #region Auth
    [Post("/api/auth/register")]
    Task Register([Body] RegisterDto registerDto);

    [Post("/api/auth/login")]
    Task<LoginResultDto> Login([Body] LoginDto loginDto);
    #endregion

    #region Juegos
    [Get("/api/juegos")]
    Task<List<JuegoDto>> GetJuegos();
    #endregion

    #region Torneos
    [Get("/api/torneos")]
    Task<List<TorneoResumenDto>> GetTorneos();

    [Get("/api/torneos/{id}")]
    Task<TorneoDetalleDto> GetTorneo(int id);
    #endregion
    
    // ... etc ...
}

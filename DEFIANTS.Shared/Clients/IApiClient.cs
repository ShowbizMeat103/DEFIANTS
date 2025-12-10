using DEFIANTS.Shared.DTOs;
using DEFIANTS.Shared.Enums; // Para RolEquipo, EstadoPago, EstadoTorneo
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

    [Get("/api/juegos/{id}")]
    Task<JuegoDto> GetJuego(int id);

    [Post("/api/admin/juegos")]
    Task<JuegoDto> CrearJuego([Body] CrearJuegoDto juegoDto);

    [Put("/api/admin/juegos/{id}")]
    Task ActualizarJuego(int id, [Body] CrearJuegoDto juegoDto);
    #endregion

    #region PerfilesJuego
    [Get("/api/perfilesjuego/misperfiles")]
    Task<List<PerfilJuegoDto>> GetMisPerfilesJuego();

    [Post("/api/perfilesjuego")]
    Task<PerfilJuegoDto> CrearPerfilJuego([Body] CrearPerfilJuegoDto perfilDto);

    [Put("/api/perfilesjuego/{id}")]
    Task ActualizarPerfilJuego(int id, [Body] ActualizarPerfilJuegoDto perfilDto);
    #endregion

    #region Equipos
    [Get("/api/equipos")]
    Task<List<EquipoResumenDto>> GetEquipos();

    [Get("/api/equipos/misequipos")]
    Task<List<MiEquipoDto>> GetMisEquipos();

    [Get("/api/equipos/{id}")]
    Task<EquipoDetalleDto> GetEquipo(int id);

    [Post("/api/equipos")]
    Task<EquipoDetalleDto> CrearEquipo([Body] CrearEquipoDto equipoDto);

    [Put("/api/equipos/{id}")]
    Task ActualizarEquipo(int id, [Body] CrearEquipoDto equipoDto);

    [Post("/api/equipos/{equipoId}/miembros")]
    Task AnadirMiembro(int equipoId, [Body] InvitarMiembroDto miembroDto);

    [Put("/api/equipos/{equipoId}/miembros/{miembroId}/rol")]
    Task ActualizarRolMiembro(int equipoId, int miembroId, [Body] ActualizarRolMiembroDto rolDto);

    [Delete("/api/equipos/{equipoId}/miembros/{miembroId}")]
    Task ExpulsarMiembro(int equipoId, int miembroId);

    [Delete("/api/equipos/{id}")]
    Task DisolverEquipo(int id);
    #endregion

    #region Torneos
    [Get("/api/torneos")]
    Task<List<TorneoResumenDto>> GetTorneos();

    [Get("/api/torneos/misinscripciones")]
    Task<List<MiInscripcionDto>> GetMisInscripciones();

    [Get("/api/torneos/{id}")]
    Task<TorneoDetalleDto> GetTorneo(int id);

    [Get("/api/torneos/{torneoId}/inscripciones")]
    Task<List<InscripcionDetalleDto>> GetInscripcionesTorneo(int torneoId);

    [Post("/api/torneos")]
    Task<TorneoDto> CrearTorneo([Body] CrearTorneoDto torneoDto);

    [Put("/api/torneos/{id}")]
    Task ActualizarTorneo(int id, [Body] CrearTorneoDto torneoDto);

    [Delete("/api/torneos/{id}")]
    Task CancelarTorneo(int id);

    [Post("/api/torneos/{torneoId}/inscripciones")]
    Task InscribirEquipo(int torneoId, [Body] InscribirEquipoDto inscripcionDto);

    [Delete("/api/torneos/{torneoId}/inscripciones/{inscripcionId}")]
    Task CancelarInscripcion(int torneoId, int inscripcionId);

    [Post("/api/torneos/{id}/iniciar")]
    Task IniciarTorneo(int id);

    [Post("/api/torneos/partidos/{partidoId}/victoria")]
    Task ReportarVictoria(int partidoId, [Body] int ganadorId);
    #endregion

    #region Partidos
    [Get("/api/partidos")] // <-- NUEVO MÉTODO
    Task<List<PartidoDto>> GetPartidos();

    [Get("/api/partidos/mispartidos")]
    Task<List<PartidoDto>> GetMisPartidos();

    [Get("/api/partidos/{id}")]
    Task<PartidoDetalleDto> GetPartido(int id);

    [Put("/api/partidos/{id}")]
    Task CorregirPartido(int id, [Body] CorregirPartidoDto partidoDto);
    #endregion

    #region Admin
    [Get("/api/admin/users")]
    Task<List<UsuarioDto>> GetUsers();

    [Get("/api/admin/users/{id}")]
    Task<UsuarioDto> GetUser(string id);

    [Post("/api/admin/assign-role")]
    Task AssignRole([Body] UpdateRoleDto updateRoleDto);

    [Delete("/api/admin/users/{id}/roles/{roleName}")]
    Task RemoveRoleFromUser(string id, string roleName);
    #endregion
}

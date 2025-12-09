namespace DEFIANTS.Server.Services;

public interface ITorneoService
{
    Task GenerarBracketsAsync(int torneoId);
    Task ReportarVictoriaAsync(int partidoId, int ganadorId);
}

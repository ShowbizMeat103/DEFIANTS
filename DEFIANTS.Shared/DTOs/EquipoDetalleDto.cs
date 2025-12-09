namespace DEFIANTS.Shared.DTOs;

public class EquipoDetalleDto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public int JuegoId { get; set; }
    public int CapitanId { get; set; } // PerfilJuegoId del capitán
    public List<MiembroEquipoDto> Miembros { get; set; } = new();
}

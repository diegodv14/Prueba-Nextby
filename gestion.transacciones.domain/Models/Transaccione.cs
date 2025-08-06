using System.Text.Json.Serialization;

namespace gestion.transacciones.domain.Models;

public partial class Transaccione
{
    public Guid Id { get; set; }

    public DateTime? Fecha { get; set; }

    public string? TipoTransaccion { get; set; }

    public Guid? ProductoId { get; set; }

    public int? Cantidad { get; set; }

    public decimal? PrecioUnitario { get; set; }

    public decimal? PrecioTotal { get; set; }

    public string? Detalle { get; set; }

    [JsonIgnore]
    public virtual Producto? Producto { get; set; } = null!;
}

using gestion.productos.domain.Models.Enums;

namespace gestion.productos.domain.Models;

public partial class Transaccione
{
    public Guid Id { get; set; }

    public DateTime? Fecha { get; set; }

    public Guid? ProductoId { get; set; }

    public TipoTransaccion TipoTransaccion { get; set; }

    public int? Cantidad { get; set; }

    public decimal? PrecioUnitario { get; set; }

    public decimal? PrecioTotal { get; set; }

    public string? Detalle { get; set; }

    public virtual Producto? Producto { get; set; }
}

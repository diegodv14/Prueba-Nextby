using gestion.transacciones.domain.Models.Enums;

namespace gestion.transacciones.domain.Dto
{
    public class RequestFiltrosTransacciones
    {
        public DateTime? fecha { get; set; }
        public TipoTransaccion? tipo { get; set; }
        public string? productoId { get; set; }
    }
}

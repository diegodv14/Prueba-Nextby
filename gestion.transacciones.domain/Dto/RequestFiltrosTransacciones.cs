using gestion.transacciones.domain.Models.Enums;

namespace gestion.transacciones.domain.Dto
{
    public class RequestFiltrosTransacciones
    {
        public DateTime? fecha { get; set; }
        public string? tipo { get; set; }
        public string? productoId { get; set; }
    }
}

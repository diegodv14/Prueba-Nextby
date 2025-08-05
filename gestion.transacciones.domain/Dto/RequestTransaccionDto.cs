namespace gestion.transacciones.domain.Dto
{
    public class RequestTransaccionDto
    {
        public Guid? ProductoId { get; set; }
        public int? Cantidad { get; set; }
        public string? Detalle { get; set; }
        public decimal? PrecioUnitario { get; set; }
        public decimal? PrecioTotal { get; set; }

    }
}

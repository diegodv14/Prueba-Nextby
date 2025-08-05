namespace gestion.productos.domain.response
{
    public class ErrorResponse
    {
        public int Code { get; set; }
        public string? Message { get; set; }
        public bool Error { get; set; }
    }
}

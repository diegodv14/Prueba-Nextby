namespace gestion.productos.domain.response
{
    public class SuccessResponse<T>(T? data, string? message, int? code)
    {
        public T? Data { get; set; } = data;
        public string? Mensaje { get; set; } = message ?? "OK";
        public int? Status { get; set; } = code ?? 200;
    }
}

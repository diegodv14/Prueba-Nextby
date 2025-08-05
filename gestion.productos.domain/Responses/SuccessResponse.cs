namespace gestion.productos.domain.response
{
    public class SuccessResponse<T>
    {
        public T? Data { get; set; }
        public string Mensaje { get; set; }
        public int Status { get; set; }

        public SuccessResponse(T? data, string? message = null, int? code = null)
        {
            Data = data;
            Mensaje = message ?? "OK";
            Status = code ?? 200;
        }
    }
}

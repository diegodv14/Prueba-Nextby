namespace gestion.transacciones.domain.exceptions
{
    public class BaseCustomException(string message, int code) : Exception(message)
    {
        public int Code { get; } = code;
    }
}

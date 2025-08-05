namespace gestion.transacciones.domain.exceptions
{
    public class BaseCustomException(string message, string stackTrace, int code) : Exception(message)
    {
        public int Code { get; } = code;
        public override string StackTrace { get; } = stackTrace;
    }
}

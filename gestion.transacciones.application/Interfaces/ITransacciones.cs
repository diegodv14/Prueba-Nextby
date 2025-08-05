namespace gestion.transacciones.application.Interfaces
{
    public interface ITransacciones
    {
        public Task GetTransacciones();

        public Task RegisterVenta();

        public Task RegisterCompra();
    }
}

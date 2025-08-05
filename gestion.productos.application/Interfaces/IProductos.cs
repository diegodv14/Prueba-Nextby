namespace gestion.productos.application.Interfaces
{
    public interface IProductos
    {
        public Task GetProductoById(Guid id);
        public Task GetAllProductos();
        public Task AddProducto();
        public Task UpdateProducto();
        public Task DeleteProducto();
    }
}

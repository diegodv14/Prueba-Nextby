using gestion.productos.domain.Dtos;
using gestion.productos.domain.Models;

namespace gestion.productos.application.Interfaces
{
    public interface IProductos
    {
        public Task<Producto> GetProductoById(Guid id);
        public Task<List<Producto>> GetAllProductos();
        public Task<Producto> AddProducto(RequestProductoDto producto);
        public Task<Producto> UpdateProducto(Guid id, RequestProductoDto producto);
        public Task<bool> DeleteProducto(Guid id);
    }
}

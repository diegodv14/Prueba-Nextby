using gestion.productos.application.Interfaces;
using gestion.productos.domain.Dtos;
using gestion.productos.domain.exceptions;
using gestion.productos.domain.Models;
using Microsoft.EntityFrameworkCore;

namespace gestion.productos.infraestructure.Repositories
{
    public class ProductoRepository(InventarioContext db) : IProductos
    {
        private readonly InventarioContext _context = db;

        public async Task<Producto> AddProducto(RequestProductoDto productoDto)
        {
            try
            {
                var producto = new Producto
                {
                    Id = Guid.NewGuid(),
                    Nombre = productoDto.Nombre,
                    Descripcion = productoDto.Descripcion,
                    Categoria = productoDto.Categoria,
                    Imagen = productoDto.Imagen,
                    Precio = productoDto.Precio,
                    Stock = productoDto.Stock
                };

                _context.Productos.Add(producto);
                await _context.SaveChangesAsync();

                return producto;
            }
            catch
            {
                throw;
            }
            ;
        }

        public async Task<bool> DeleteProducto(Guid id)
        {
            try
            {
                var producto = await _context.Productos.FindAsync(id) ?? throw new BaseCustomException($"El producto con id {id} no existe", 404);
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<Producto>> GetAllProductos()
        {
            try
            {
                var res = await _context.Productos.ToListAsync();
                return res;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Producto> GetProductoById(Guid id)
        {
            try
            {
                var producto = await _context.Productos.FindAsync(id) ?? throw new BaseCustomException($"El producto con id {id} no existe", 404);
                return producto;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Producto> UpdateProducto(Guid id, RequestProductoDto productoDto)
        {
            try
            {
                var producto = await _context.Productos.FindAsync(id) ?? throw new BaseCustomException($"El producto con id {id} no existe", 404);
                producto.Nombre = productoDto.Nombre;
                producto.Descripcion = productoDto.Descripcion;
                producto.Categoria = productoDto.Categoria;
                producto.Imagen = productoDto.Imagen;
                producto.Precio = productoDto.Precio;
                producto.Stock = productoDto.Stock;

                _context.Productos.Update(producto);
                await _context.SaveChangesAsync();
                return producto;
            }
            catch
            {
                throw;
            }
        }
    }
}

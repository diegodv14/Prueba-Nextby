using gestion.productos.domain.Dtos;
using gestion.transacciones.application.Interfaces;
using gestion.transacciones.domain.Dto;
using gestion.transacciones.domain.exceptions;
using gestion.transacciones.domain.Models;
using gestion.transacciones.domain.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace gestion.transacciones.infraestructure.Repositories
{
    public class TransaccionesRepository(InventarioContext db, HttpClient httpClient, IConfiguration config) : ITransacciones
    {
        private readonly InventarioContext _context = db;
        private readonly HttpClient _httpClient = httpClient;
        private readonly IConfiguration _config = config;

        public async Task<Transaccione> AddTransaccion(TipoTransaccion tipo, RequestTransaccionDto data)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteTransaccion(Guid id)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var transaccion = await _context.Transacciones
                    .Include(t => t.Producto)
                    .FirstOrDefaultAsync(t => t.Id == id)
                    ?? throw new BaseCustomException($"No existe una transacción con el id {id}", 404);

                // Regresarle / Disminuirle al producto la cantidad de la transacción según su tipo.
                // Se puede realizar en la misma función pero para simular un ambiente de microservicios empleare el otro microservicio de producto.
                var devolucion = transaccion.Cantidad;

                var request = new RequestProductoDto()
                {
                    Stock = transaccion.TipoTransaccion.Equals(TipoTransaccion.venta) ? (transaccion.Producto?.Stock + devolucion) : (transaccion.Producto?.Stock - devolucion)
                };
                var urlProducto = _config.GetConnectionString("producto_service") ?? throw new BaseCustomException("La uri del microservicio de productos no esta configurada",500);
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var res = await _httpClient.PutAsync($"{urlProducto}/api/Productos/EditarProducto?id={transaccion.ProductoId}", content);
                res.EnsureSuccessStatusCode();  

                _context.Transacciones.Remove(transaccion);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<Transaccione>> GetTransacciones()
        {
            try
            {
                var transacciones = await _context.Transacciones.ToListAsync();
                return transacciones;
            }
            catch
            {
                throw;
            }
        }

        public Task<Transaccione> UpdateTransaccion(Guid id, RequestTransaccionDto data)
        {
            throw new NotImplementedException();
        }
    }
}

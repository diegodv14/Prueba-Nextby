using gestion.productos.domain.Dtos;
using gestion.productos.domain.response;
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


        // La logica referente a los productos se puede realizar en la misma función pero para simular un ambiente de microservicios empleare el otro microservicio de producto.

        public async Task<Transaccione> AddTransaccion(TipoTransaccion tipo, RequestTransaccionDto data)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {

                // Obtener el producto
                var urlProducto = _config.GetConnectionString("producto_service") ?? throw new BaseCustomException("La uri del microservicio de productos no esta configurada", 500);
                var res = await _httpClient.GetAsync($"{urlProducto}/api/Productos/{data.ProductoId}");
                var content = await res.Content.ReadAsStringAsync();
                var json = JsonSerializer.Deserialize<SuccessResponse<Producto>>(content);


                if (json == null || json.Status == 500)
                {
                    throw new BaseCustomException($"Hubo un error al verificar la existencia del producto con id {data.ProductoId}", 500);
                }
                if (json.Status == 404)
                {
                    throw new BaseCustomException($"El producto con id {data.ProductoId} no existe", 404);
                }

                var producto = json.Data;

                var request = new RequestProductoDto();

                // Realizar actualización del producto en base del tipo de transacción
                switch (tipo)
                {
                    case TipoTransaccion.compra:
                        {
                            request.Stock = producto!.Stock + data.Cantidad;
                            break;
                        }
                    case TipoTransaccion.venta:
                        {
                            request.Stock = producto!.Stock - data.Cantidad;   
                            break;
                        }
                    default:
                        {
                            throw new BaseCustomException("Tipo de transacción no válido", 400);
                        }
                }
                var jsonActualizarProducto = JsonSerializer.Serialize(request);
                var contentActualizarProducto = new StringContent(jsonActualizarProducto, Encoding.UTF8, "application/json");
                var resActualizarProducto = await _httpClient.PutAsync($"{urlProducto}/api/Productos?id={data.ProductoId}", contentActualizarProducto);
                resActualizarProducto.EnsureSuccessStatusCode();

                var newTransaccion = new Transaccione()
                {
                    TipoTransaccion = tipo,
                    Cantidad = data.Cantidad,
                    Detalle = data.Detalle,
                    PrecioTotal = data.PrecioTotal,
                    PrecioUnitario = data.PrecioUnitario
                };

                _context.Transacciones.Add(newTransaccion);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return newTransaccion;
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

                var devolucion = transaccion.Cantidad;

                var request = new RequestProductoDto()
                {
                    Stock = transaccion.TipoTransaccion.Equals(TipoTransaccion.venta) ? (transaccion.Producto?.Stock + devolucion) : (transaccion.Producto?.Stock - devolucion)
                };
                var urlProducto = _config.GetConnectionString("producto_service") ?? throw new BaseCustomException("La uri del microservicio de productos no esta configurada",500);
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var res = await _httpClient.PutAsync($"{urlProducto}/api/Productos?id={transaccion.ProductoId}", content);
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

        public async Task<List<Transaccione>> GetTransacciones(RequestFiltrosTransacciones filtros)
        {
            try
            {
                var query = _context.Transacciones
                    .Include(t => t.Producto)
                    .AsQueryable();
                if (filtros.fecha.HasValue)
                {
                    query = query.Where(t => t.Fecha == filtros.fecha.Value);
                }
                if (!string.IsNullOrEmpty(filtros.productoId))
                {
                    var idProducto = Guid.TryParse(filtros.productoId, out Guid id) ? id : throw new BaseCustomException("El producto id proporcionado no es valido", 400);
                    query = query.Where(t => t.ProductoId == idProducto);
                }
                if (filtros.tipo.HasValue)
                {
                    query = query.Where(t => t.TipoTransaccion == filtros.tipo.Value);
                }
                var transacciones = await query
                    .OrderByDescending(t => t.Fecha)
                    .ToListAsync();

                return transacciones;
            }
            catch
            {
                throw;
            }
        }


        public async Task<Transaccione> UpdateTransaccion(Guid id, RequestTransaccionDto data)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var transaccion = await _context.Transacciones
                    .Include(t => t.Producto)
                    .FirstOrDefaultAsync(t => t.Id == id)
                    ?? throw new BaseCustomException($"La transacción con id {id} no existe", 404);

                var urlProducto = _config.GetConnectionString("producto_service")
                    ?? throw new BaseCustomException("La uri del microservicio de productos no está configurada", 500);

                var producto = transaccion.Producto;

                var diferenciaCantidad = data.Cantidad - transaccion.Cantidad;

                // Realizar actualización del producto en base del tipo de transacción
                var nuevoStock = transaccion.TipoTransaccion.Equals(TipoTransaccion.compra)
                    ? producto!.Stock + diferenciaCantidad
                    : producto!.Stock - diferenciaCantidad;

                var requestProducto = new RequestProductoDto
                {
                    Stock = nuevoStock
                };

                var jsonActualizar = JsonSerializer.Serialize(requestProducto);
                var contentActualizar = new StringContent(jsonActualizar, Encoding.UTF8, "application/json");

                var resActualizar = await _httpClient.PutAsync($"{urlProducto}/api/Productos?id={transaccion.ProductoId}", contentActualizar);
                resActualizar.EnsureSuccessStatusCode();
                transaccion.Cantidad = data.Cantidad;
                transaccion.Detalle = data.Detalle;
                transaccion.PrecioUnitario = data.PrecioUnitario;
                transaccion.PrecioTotal = data.PrecioTotal;

                _context.Transacciones.Update(transaccion);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return transaccion;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

    }
}

using gestion.productos.domain.Dtos;
using gestion.productos.domain.response;
using gestion.transacciones.application.Interfaces;
using gestion.transacciones.domain.Dto;
using gestion.transacciones.domain.exceptions;
using gestion.transacciones.domain.Models;
using gestion.transacciones.domain.Models.Enums;
using gestion.transacciones.domain.response;
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
        private readonly JsonSerializerOptions _options = new()
        {
            PropertyNameCaseInsensitive = true
        };


        // La logica referente a los productos se puede realizar en la misma función pues estamos conectados a la misma db
        // pero para simular un ambiente de microservicios empleare el otro microservicio de producto.

        public async Task<Transaccione> AddTransaccion(string tipo, RequestTransaccionDto data)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {

                // Obtener el producto
                var urlProducto = _config.GetConnectionString("producto_service") ?? throw new BaseCustomException("La uri del microservicio de productos no esta configurada", 500);
                var res = await _httpClient.GetAsync($"{urlProducto}/api/Productos/{data.ProductoId}");
                var content = await res.Content.ReadAsStringAsync();
                Producto producto;
                if (res.IsSuccessStatusCode)
                {
                    var success = JsonSerializer.Deserialize<SuccessResponse<Producto>>(content, _options);

                    if (success == null || success.Data == null)
                    {
                        throw new BaseCustomException("La respuesta no contiene datos del producto", 500);
                    }

                    producto = success.Data;
                }
                else
                {
                    var error = JsonSerializer.Deserialize<ErrorResponse>(content, _options);

                    if (error == null || error.Code == 404)
                    {
                        throw new BaseCustomException($"El producto con id {data.ProductoId} no existe", 404);
                    }

                    if (error == null || error.Code == 500)
                    {
                        throw new BaseCustomException($"Hubo un error al verificar la existencia del producto con id {data.ProductoId}", 500);
                    }

                    throw new BaseCustomException(error.Message ?? "Error interno al verificar el producto", error.Code);
                }

                var request = new RequestProductoDto();

                // Realizar actualización del producto en base del tipo de transacción
                var nuevoStock = tipo.ToLower().Trim() switch
                {
                    "compra" => (producto.Stock ?? 0) + (data.Cantidad ?? 0),
                    "venta" => (producto.Stock ?? 0) - (data.Cantidad ?? 0),
                    _ => throw new BaseCustomException("Tipo de transacción no válido, solo se permiten compra y venta", 400),
                };
                if (nuevoStock < 0)
                {
                    throw new BaseCustomException("Stock insuficiente para realizar la operación", 400);
                }

                request.Stock = nuevoStock;

                var jsonActualizarProducto = JsonSerializer.Serialize(request);
                var contentActualizarProducto = new StringContent(jsonActualizarProducto, Encoding.UTF8, "application/json");
                var resActualizarProducto = await _httpClient.PutAsync($"{urlProducto}/api/Productos?id={data.ProductoId}", contentActualizarProducto);
                var resContent = await resActualizarProducto.Content.ReadAsStringAsync();

                if (!resActualizarProducto.IsSuccessStatusCode)
                {
                    var error = JsonSerializer.Deserialize<ErrorResponse>(resContent, _options);

                    if (error == null || error.Code == 404)
                    {
                        throw new BaseCustomException($"El producto con id {data.ProductoId} no existe", 404);
                    }

                    if (error == null || error.Code == 500)
                    {
                        throw new BaseCustomException($"Hubo un error al actualizar el stock del producto con id {data.ProductoId}", 500);
                    }

                    throw new BaseCustomException(error.Message ?? "Error interno al actualizar el producto", error.Code);
                }

                var newTransaccion = new Transaccione()
                {
                    TipoTransaccion = tipo,
                    Cantidad = data.Cantidad,
                    ProductoId = producto.Id,
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

                int nuevoStock = transaccion.TipoTransaccion?.ToLower().Trim() == "venta"
                    ? (transaccion.Producto?.Stock + devolucion ?? 0)
                    : (transaccion.Producto?.Stock - devolucion ?? 0);

                if (nuevoStock < 0)
                {
                    throw new BaseCustomException("Stock insuficiente para realizar la operación", 400);
                }

                var request = new RequestProductoDto()
                {
                    Stock = nuevoStock
                };
                var urlProducto = _config.GetConnectionString("producto_service") ?? throw new BaseCustomException("La uri del microservicio de productos no esta configurada", 500);
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var res = await _httpClient.PutAsync($"{urlProducto}/api/Productos?id={transaccion.ProductoId}", content);
                var resContent = await res.Content.ReadAsStringAsync();

                if (!res.IsSuccessStatusCode)
                {
                    var error = JsonSerializer.Deserialize<ErrorResponse>(resContent, _options);

                    if (error == null || error.Code == 404)
                    {
                        throw new BaseCustomException($"El producto con id {transaccion.ProductoId} no existe", 404);
                    }

                    if (error == null || error.Code == 500)
                    {
                        throw new BaseCustomException($"Hubo un error al actualizar el stock del producto con id {transaccion.ProductoId}", 500);
                    }

                    throw new BaseCustomException(error.Message ?? "Error interno al actualizar el producto", error.Code);
                }

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
                if (!string.IsNullOrEmpty(filtros.tipo))
                {
                    if(filtros.tipo.ToLower().Trim() != "compra" && filtros.tipo.ToLower().Trim() != "venta")
                    {
                        throw new BaseCustomException("Tipo de transacción no válido para filtrar, solo se permiten compra y venta", 400);
                    }

                    query = query.Where(t => t.TipoTransaccion == filtros.tipo);
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

                var nuevoStock = transaccion.TipoTransaccion?.ToLower().Trim() == "compra"
                    ? producto!.Stock + diferenciaCantidad
                    : producto!.Stock - diferenciaCantidad;

                if (nuevoStock < 0)
                {
                    throw new BaseCustomException("Stock insuficiente para realizar la operación", 400);
                }

                var requestProducto = new RequestProductoDto
                {
                    Stock = nuevoStock
                };

                var jsonActualizar = JsonSerializer.Serialize(requestProducto);
                var contentActualizar = new StringContent(jsonActualizar, Encoding.UTF8, "application/json");

                var resActualizar = await _httpClient.PutAsync($"{urlProducto}/api/Productos?id={transaccion.ProductoId}", contentActualizar);
                var resContent = await resActualizar.Content.ReadAsStringAsync();

                if (!resActualizar.IsSuccessStatusCode)
                {
                    var error = JsonSerializer.Deserialize<ErrorResponse>(resContent, _options);

                    if (error == null || error.Code == 404)
                    {
                        throw new BaseCustomException($"El producto con id {transaccion.ProductoId} no existe", 404);
                    }

                    if (error == null || error.Code == 500)
                    {
                        throw new BaseCustomException($"Hubo un error al actualizar el stock del producto con id {transaccion.ProductoId}", 500);
                    }

                    throw new BaseCustomException(error.Message ?? "Error interno al actualizar el producto", error.Code);
                }

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

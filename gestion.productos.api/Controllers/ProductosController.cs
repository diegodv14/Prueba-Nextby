using gestion.productos.application.Interfaces;
using gestion.productos.domain.Dtos;
using gestion.productos.domain.Models;
using gestion.productos.domain.response;
using Microsoft.AspNetCore.Mvc;

namespace gestion.productos.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController(IProductos repository) : ControllerBase
    {
        private readonly IProductos _repository = repository;

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SuccessResponse<List<Producto>>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<SuccessResponse<List<Producto>>> ListarProductos()
        {
            var res = await _repository.GetAllProductos();
            return new SuccessResponse<List<Producto>>(res, "Lista de productos devuelta exitosamente", 200);
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SuccessResponse<Producto>), 201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<SuccessResponse<Producto>> CrearProducto([FromBody] RequestProductoDto producto)
        {
            var res = await _repository.AddProducto(producto);
            return new SuccessResponse<Producto>(res, "Producto creado exitosamente", 201);
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SuccessResponse<Producto>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<SuccessResponse<Producto>> ObtenerProductoPorId(string id)
        {
            var res = await _repository.GetProductoById(Guid.Parse(id));
            return new SuccessResponse<Producto>(res, $"Producto obtenido por id {id} exitosamente", 200);
        }

        [HttpPut]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SuccessResponse<Producto>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<SuccessResponse<Producto>> EditarProducto([FromQuery] string id, [FromBody] RequestProductoDto producto)
        {
            var res = await _repository.UpdateProducto(Guid.Parse(id), producto);
            return new SuccessResponse<Producto>(res, $"Producto actualizado exitosamente", 200);
        }

        [HttpDelete]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SuccessResponse<bool>), 204)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<SuccessResponse<bool>> EliminarProducto([FromQuery] string id)
        {
            var res = await _repository.DeleteProducto(Guid.Parse(id));
            return new SuccessResponse<bool>(res, "Producto eliminado exitosamente", 204);
        }
    }
}

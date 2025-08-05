using gestion.productos.application.Interfaces;
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
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<IActionResult> ListarProductos()
        {
            var res = await _repository.GetAllProductos();
            return Ok(new List<object>());
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<IActionResult> CrearProducto([FromBody] object producto)
        {
            var res = await _repository.AddProducto();
            return CreatedAtAction(nameof(CrearProducto), new { id = 1 }, producto);
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<IActionResult> ObtenerProductoPorId(string id)
        {
            var res = await _repository.GetProductoById(Guid.Parse(id));
            return Ok(new { Id = id });
        }

        [HttpPut("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<IActionResult> EditarProducto(int id, [FromBody] object producto)
        {
            var res = await _repository.UpdateProducto();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            var res = await _repository.DeleteProducto();
            return NoContent();
        }
    }
}

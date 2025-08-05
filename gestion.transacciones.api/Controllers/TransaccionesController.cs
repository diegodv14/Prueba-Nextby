using gestion.productos.domain.response;
using gestion.transacciones.application.Interfaces;
using gestion.transacciones.domain.Dto;
using gestion.transacciones.domain.Models;
using gestion.transacciones.domain.Models.Enums;
using gestion.transacciones.domain.response;
using Microsoft.AspNetCore.Mvc;

namespace gestion.transacciones.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransaccionesController(ITransacciones repository) : ControllerBase
    {
        private readonly ITransacciones _repository = repository;

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SuccessResponse<List<Transaccione>>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<SuccessResponse<List<Transaccione>>> ListarTransacciones()
        {
            var res = await _repository.GetTransacciones();
            return new SuccessResponse<List<Transaccione>>(res, "Lista de Transacciones devuelta exitosamente", 200);
        }

        [HttpPost("{tipoTransaccion}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SuccessResponse<Transaccione>), 201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<SuccessResponse<Transaccione>> RegistrarTransaccion([FromRoute] TipoTransaccion tipoTransaccion, RequestTransaccionDto data)
        {
            var res = await _repository.AddTransaccion(tipoTransaccion, data);
            return new SuccessResponse<Transaccione>(res, $"Transacción de {tipoTransaccion} creada exitosamente", 201);
        }

        [HttpPut]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SuccessResponse<Transaccione>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<SuccessResponse<Transaccione>> ActualizarTransaccion([FromQuery] string id, [FromBody] RequestTransaccionDto data)
        {
            var res = await _repository.UpdateTransaccion(Guid.Parse(id), data);
            return new SuccessResponse<Transaccione>(res, $"Transacción actualizada exitosamente", 200);
        } 

        [HttpDelete]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SuccessResponse<bool>), 204)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<SuccessResponse<bool>> EliminarTransaccion(string id)
        {
            bool res = await _repository.DeleteTransaccion(Guid.Parse(id));
            return new SuccessResponse<bool>(res, "Transacción eliminada exitosamente", 204);
        }
    }
}

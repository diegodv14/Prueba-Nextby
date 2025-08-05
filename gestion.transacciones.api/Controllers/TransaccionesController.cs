using gestion.transacciones.application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace gestion.transacciones.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransaccionesController(ITransacciones repository): ControllerBase
    {
        private readonly ITransacciones _repository = repository;


        [HttpGet]
        [Produces("application/json")]
        public async Task ListarTransacciones()
        {

        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task RegistrarVenta()
        {
            await _repository.RegisterVenta();
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task RegistrarCompra()
        {
            await _repository.RegisterCompra();
        }
    }
}

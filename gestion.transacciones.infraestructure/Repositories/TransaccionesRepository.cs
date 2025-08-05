using gestion.transacciones.application.Interfaces;
using gestion.transacciones.domain.Models;

namespace gestion.transacciones.infraestructure.Repositories
{
    public class TransaccionesRepository(InventarioContext db) : ITransacciones
    {
        private readonly InventarioContext _context = db;
        public Task GetTransacciones()
        {
            throw new NotImplementedException();
        }

        public Task RegisterCompra()
        {
            throw new NotImplementedException();
        }

        public Task RegisterVenta()
        {
            throw new NotImplementedException();
        }
    }
}

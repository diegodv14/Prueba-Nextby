using gestion.transacciones.domain.Dto;
using gestion.transacciones.domain.Models;
using gestion.transacciones.domain.Models.Enums;

namespace gestion.transacciones.application.Interfaces
{
    public interface ITransacciones
    {
        public Task<List<Transaccione>> GetTransacciones(RequestFiltrosTransacciones data);
        public Task<Transaccione> AddTransaccion(string tipoTransaccion, RequestTransaccionDto data);
        public Task<Transaccione> UpdateTransaccion(Guid id, RequestTransaccionDto data);
        public Task<bool> DeleteTransaccion(Guid id);
    }
}

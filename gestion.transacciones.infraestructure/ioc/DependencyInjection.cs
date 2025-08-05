using gestion.transacciones.application.Interfaces;
using gestion.transacciones.infraestructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace gestion.transacciones.infraestructure.ioc
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfraestructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITransacciones, TransaccionesRepository>();
            return services;
        }
    }
}
using gestion.transacciones.application.Interfaces;
using gestion.transacciones.infraestructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using gestion.transacciones.domain.Models;

namespace gestion.transacciones.infraestructure.ioc
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfraestructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<InventarioContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("db"))
            );
            services.AddHttpClient();
            services.AddScoped<ITransacciones, TransaccionesRepository>();
            return services;
        }
    }
}
using gestion.productos.application.Interfaces;
using gestion.productos.infraestructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace gestion.productos.infraestructure.ioc
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfraestructure(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddScoped<IProductos, ProductoRepository>();
            return services;
        }
    }
}
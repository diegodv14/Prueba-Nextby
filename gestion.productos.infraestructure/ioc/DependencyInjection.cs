using gestion.productos.application.Interfaces;
using gestion.productos.domain.Models;
using gestion.productos.infraestructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace gestion.productos.infraestructure.ioc
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfraestructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<InventarioContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("db"))
            );
            services.AddScoped<IProductos, ProductoRepository>();
            return services;
        }
    }
}
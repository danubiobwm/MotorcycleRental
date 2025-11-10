using Domain.Interfaces;
using Infra.Data;
using Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infra
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddScoped<IMotorcycleRepository, MotorcycleRepository>();
            services.AddScoped<ICourierRepository, CourierRepository>();
            services.AddScoped<IRentalRepository, RentalRepository>();

            return services;
        }
    }
}

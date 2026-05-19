using CompanyCatalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CompanyCatalog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database")
                               ?? throw new InvalidOperationException("Db connection string bulunamadı");

        services.AddDbContext<ApplicationDbContext>(options => { options.UseNpgsql(connectionString); });

        return services;
    }
}
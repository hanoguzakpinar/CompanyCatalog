using CompanyCatalog.Application.Abstractions.Authentication;
using CompanyCatalog.Application.Abstractions.Persistence;
using CompanyCatalog.Infrastructure.Authentication;
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

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IUserRepository, IUserRepository>();

        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtService, JwtService>();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        return services;
    }
}
using FlightServiceAPI.Application.Common.Interfaces;
using FlightServiceAPI.Infrastructure.Cache;
using FlightServiceAPI.Infrastructure.Data;
using FlightServiceAPI.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlightServiceAPI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DatabaseConnection");
        
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DatabaseConnection' not found.");
        }
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        
        services.AddScoped<ApplicationDbContextInitializer>();
        
        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        services.AddMemoryCache();
        services.AddScoped<ICacheService, MemoryCacheService>();
        
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        
        return services;
    }
}
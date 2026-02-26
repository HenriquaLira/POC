namespace UserAPI.API.Extensions;

using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UserAPI.Core.Application.Interfaces;
using UserAPI.Core.Application.Services;
using UserAPI.Core.Infrastructure.Data;
using UserAPI.Core.Infrastructure.Repositories;

/// <summary>
/// Extension methods for service registration
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Register core services and repositories
    /// </summary>
    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();

        // Register application services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();

        // Register utility services
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        // Register JWT token service with config-driven Issuer/Audience
        var secretKey = configuration["Jwt:SecretKey"]
            ?? throw new InvalidOperationException("JWT secret key not found in configuration");
        var issuer = configuration["Jwt:Issuer"] ?? "UserAPI";
        var audience = configuration["Jwt:Audience"] ?? "UserAPIClient";
        var expirationMinutes = int.Parse(configuration["Jwt:ExpirationMinutes"] ?? "60");
        services.AddScoped<IJwtTokenService>(_ => new JwtTokenService(secretKey, issuer, audience, expirationMinutes));

        // Register validators and wire up automatic validation pipeline
        services.AddValidatorsFromAssemblyContaining(typeof(CreateUserValidator), ServiceLifetime.Scoped);

        return services;
    }

    /// <summary>
    /// Register database context
    /// </summary>
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection string not found in configuration");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString)
        );

        return services;
    }
}

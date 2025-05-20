using System.Data;
using Api.Dtos;
using Api.PasswordHashers;
using Api.Services;
using Api.Services.Interfaces;
using Api.Validators;
using FluentValidation;
using Microsoft.Data.SqlClient;

namespace Api.Configurations;

public static class DependencyInjection
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {   
        string? connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("No connection string found.");
        }

        try
        {   
            var connection = new SqlConnection(connectionString);
            connection.Open();
            connection.Close();
            
            services.AddSingleton<IDbConnection>(_ => new SqlConnection(connectionString));
            return services;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Could not connect to database '{connectionString}'.", ex);
        }
    }
    
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }

    public static IServiceCollection AddDependencies(this IServiceCollection services)
    {
        services.AddSingleton<IPasswordHasher, Argon2PasswordHasher>();
        return services;
    }
    
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<UpdateUserDto>, UpdateUserValidator>();
        return services;
    }
}

using System.Data;
using System.Text;
using Api.Dtos;
using Api.PasswordHashers;
using Api.Services;
using Api.Services.Interfaces;
using Api.Validators;
using FluentValidation;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

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
        services.AddScoped<IValidator<RegisterUserDto>, RegisterUserValidator>();
        services.AddScoped<IValidator<LoginUserDto>, LoginUserValidator>();
        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {   
        string jwtKey = GetEnvironmentVariableOrThrow(configuration, "JWT_KEY");
        string jwtIssuer = GetEnvironmentVariableOrThrow(configuration, "JWT_ISSUER");
        string jwtAudience = GetEnvironmentVariableOrThrow(configuration, "JWT_AUDIENCE");
        
        services.AddAuthentication("Bearer").AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudiences = jwtAudience.Split(",").Select(j => j.Trim()).ToArray(),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ClockSkew = TimeSpan.Zero
            };
        });
        return services;
    }
    
    private static string GetEnvironmentVariableOrThrow(IConfiguration configuration, string key)
    {
        var value = configuration.GetValue<string>(key);
        if (string.IsNullOrWhiteSpace(value))
        {   
            throw new InvalidOperationException($"Environment variable '{key}' is required but was either empty or not found.");
        }
        return value;
    }
}

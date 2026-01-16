using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace FCG.Lib.Shared.Infrastructure.DependencyInjection;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "Authentication:JwtBearer")
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            configuration.GetSection(sectionName).Bind(options);
            
            // Validação do Authority
            if (string.IsNullOrEmpty(options.Authority))
            {
                throw new ArgumentException($"JWT Authority is required. Configure '{sectionName}:Authority' in appsettings.json");
            }

            // Configurações padrão que não vêm do appsettings
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidIssuer = options.Authority,
                ValidateLifetime = true,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };
            options.MetadataAddress = $"{options.Authority}/.well-known/openid-configuration";
            options.SaveToken = true;
            options.RefreshOnIssuerKeyNotFound = true;
            options.BackchannelTimeout = TimeSpan.FromSeconds(30);

            // Logging events for debugging and monitoring
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
                    logger.LogError("Authentication failed: {Message}", context.Exception.Message);
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
                    var username = context.Principal?.FindFirst("username")?.Value 
                                ?? context.Principal?.FindFirst("sub")?.Value 
                                ?? "Unknown";
                    logger.LogInformation("Token validated for user: {User}", username);
                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }

    public static IServiceCollection AddDefaultAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireClaim("cognito:groups", "Admin"));
            options.AddPolicy("UserOrAdmin", policy =>
                policy.RequireAuthenticatedUser());
        });

        return services;
    }
}

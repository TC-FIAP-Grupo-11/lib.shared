using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Lib.Shared.Infrastructure.DependencyInjection;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "AWS")
    {
        var region = configuration[$"{sectionName}:Region"];
        var userPoolId = configuration[$"{sectionName}:Cognito:UserPoolId"];
        var clientId = configuration[$"{sectionName}:Cognito:ClientId"];
        var authority = $"https://cognito-idp.{region}.amazonaws.com/{userPoolId}";

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Authority = authority;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidIssuer = authority,
                ValidateLifetime = true,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };
            options.MetadataAddress = $"{authority}/.well-known/openid-configuration";
            options.RequireHttpsMetadata = true;
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

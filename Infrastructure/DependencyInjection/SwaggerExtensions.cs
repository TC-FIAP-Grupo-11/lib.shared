using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace FCG.Lib.Shared.Infrastructure.DependencyInjection;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerWithJwt(
        this IServiceCollection services,
        string apiTitle,
        string apiVersion = "v1")
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(apiVersion, new OpenApiInfo
            {
                Title = apiTitle,
                Version = apiVersion
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header usando Bearer scheme. Exemplo: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}

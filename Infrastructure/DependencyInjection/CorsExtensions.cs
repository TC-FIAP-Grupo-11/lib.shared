using Microsoft.Extensions.DependencyInjection;

namespace FCG.Lib.Shared.Infrastructure.DependencyInjection;

public static class CorsExtensions
{
    public static IServiceCollection AddDefaultCors(
        this IServiceCollection services,
        string policyName = "AllowAll")
    {
        services.AddCors(options =>
        {
            options.AddPolicy(policyName, policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        return services;
    }

    public static IServiceCollection AddCustomCors(
        this IServiceCollection services,
        string policyName,
        Action<Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicyBuilder> configurePolicy)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(policyName, configurePolicy);
        });

        return services;
    }
}

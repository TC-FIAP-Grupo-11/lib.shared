using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FCG.Lib.Shared.Infrastructure.DependencyInjection;

public static class DatabaseExtensions
{
    public static IServiceCollection AddSqlServerDatabase<TContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionStringName = "DefaultConnection",
        int maxRetryCount = 5,
        int maxRetryDelaySeconds = 10,
        int commandTimeoutSeconds = 30) where TContext : DbContext
    {
        var connectionString = configuration.GetConnectionString(connectionStringName);

        services.AddDbContext<TContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: maxRetryCount,
                    maxRetryDelay: TimeSpan.FromSeconds(maxRetryDelaySeconds),
                    errorNumbersToAdd: null);

                sqlOptions.CommandTimeout(commandTimeoutSeconds);
            });

            #if DEBUG
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
            #endif
        });

        return services;
    }
}

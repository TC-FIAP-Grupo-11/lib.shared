using Amazon.XRay.Recorder.Handlers.AspNetCore;
using Microsoft.AspNetCore.Builder;

namespace FCG.Lib.Shared.Infrastructure.DependencyInjection;

public static class ObservabilityExtensions
{
    /// <summary>
    /// Habilita AWS X-Ray para rastreamento distribuído.
    /// Registra automaticamente todas as requisições HTTP como segmentos no CloudWatch X-Ray.
    /// </summary>
    /// <param name="app">O pipeline da aplicação.</param>
    /// <param name="serviceName">Nome do serviço exibido no mapa de serviços do X-Ray.</param>
    public static IApplicationBuilder UseXRayTracing(this IApplicationBuilder app, string serviceName)
    {
        app.UseXRay(serviceName);
        return app;
    }
}

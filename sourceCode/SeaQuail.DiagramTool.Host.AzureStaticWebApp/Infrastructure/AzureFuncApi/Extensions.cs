using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SeaQuail.DiagramTool.Domain.Shared.Services;
using SeaQuail.DiagramTool.Host.AzureSwa.Infrastructure.AzureFuncApi.Default;

namespace SeaQuail.DiagramTool.Host.AzureSwa.Infrastructure.AzureFuncApi;

public static class Extensions
{
    public static IServiceCollection AddApi<TApi>(this IServiceCollection services, string? explicitPath = null) where TApi : class
        => services.AddTransient<TApi>()
            .AddSingleton(_ => FuncApiAutoEndpointProvider.CreateApiPath<TApi>(explicitPath));

    public static IServiceCollection AddDefaultFuncApis(this IServiceCollection services, string pathPrefix = "")
    {
        return services.AddSingleton(p =>
            {
                var methodAdapter = new AutoMethodInfoRequestHandlerBuilder();
                var apiPaths = p.GetServices<FuncApiAutoEndpointProvider.ApiPath>()
                    .Select(p => p.Repath(string.Join("/", [pathPrefix, p.Path])));
                var endpointProvider = new FuncApiAutoEndpointProvider(apiPaths, methodAdapter);
                var reqHandler = new FuncApiEndpointRequestHandler(endpointProvider);

                return reqHandler;
            })
            .AddTransient(p => 
                new FuncApiExceptionHandler(
                    p.GetRequiredService<FuncApiEndpointRequestHandler>(),
                    p.GetRequiredService<ILogger<FuncApiExceptionHandler>>()
                    )
                )
            .AddScoped(p => new FuncApiUserContextProvider(p.GetRequiredService<FuncApiExceptionHandler>()))
            .AddTransient<IFuncApiRequestHandler>(p => p.GetRequiredService<FuncApiUserContextProvider>())
            .AddTransient<IUserProvider>(p => p.GetRequiredService<FuncApiUserContextProvider>())
            ;
    }
}

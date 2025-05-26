using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.DependencyInjection;

namespace SeaQuail.DiagramTool.Host.AzureSwa.Infrastructure.AzureFuncApi;

public class FunctionHttpApiEndpoint
{
    [Function("RouteApiRequest")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "{*path}")] HttpRequestData req,
        string path,
        FunctionContext ctx
        )
    {
        var handler = ctx.InstanceServices.GetRequiredService<IFuncApiRequestHandler>();
        return await handler.HandleRequest(req);
    }
}

internal interface IFuncApiRequestHandler
{
    Task<HttpResponseData> HandleRequest(HttpRequestData req);
}

using Microsoft.Azure.Functions.Worker.Http;
using SeaQuail.DiagramTool.Domain.Shared.Exceptions;

namespace SeaQuail.DiagramTool.Host.AzureSwa.Infrastructure.AzureFuncApi.Default
{

    internal class FuncApiEndpointRequestHandler : IFuncApiRequestHandler
    {
        private readonly Lazy<IReadOnlyDictionary<string, RequestHandler>> apiReqHandlers;

        public FuncApiEndpointRequestHandler(
            IFuncApiEndpointProvider endpointProvider
            )
        {
            apiReqHandlers = new(() =>
            {
                return endpointProvider.GetHandlers()
                    .ToDictionary(
                        p => p.path, 
                        p => p.handler, 
                        StringComparer.OrdinalIgnoreCase
                        )
                    .AsReadOnly();
            });
        }

        public async Task<HttpResponseData> HandleRequest(HttpRequestData req)
        {
            if (!apiReqHandlers.Value.TryGetValue(req.Url.AbsolutePath.Trim('/'), out var handler))
                throw ServiceException.NotFound($"idk about '{req.Url.AbsolutePath}'");

            return await handler(req);
        }
    }

    internal delegate Task<HttpResponseData> RequestHandler(HttpRequestData req);
    internal interface IFuncApiEndpointProvider
    {
        IEnumerable<(string path, RequestHandler handler)> GetHandlers();
    }
}

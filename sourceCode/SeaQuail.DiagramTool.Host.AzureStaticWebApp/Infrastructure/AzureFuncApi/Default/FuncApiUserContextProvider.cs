using Microsoft.Azure.Functions.Worker.Http;
using SeaQuail.DiagramTool.Domain.Shared.Services;

namespace SeaQuail.DiagramTool.Host.AzureSwa.Infrastructure.AzureFuncApi.Default
{
    internal class FuncApiUserContextProvider : IFuncApiRequestHandler, IUserProvider
    {
        private readonly IFuncApiRequestHandler nextHandler;
        private (string email, string externalId)? user = null;

        public FuncApiUserContextProvider(IFuncApiRequestHandler nextHandler)
        {
            this.nextHandler = nextHandler;
        }

        public Task<HttpResponseData> HandleRequest(HttpRequestData req)
        {
            user = GetUserFromRequest(req);
            return nextHandler.HandleRequest(req);
        }

        public (string email, string externalId)? GetCurrentUser()
        {
            return user;
        }

        private (string email, string externalId)? GetUserFromRequest(HttpRequestData req)
        {
            return ("fake", "00000000-0000-0000-0000-000000000001");
        }
    }
}

using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SeaQuail.DiagramTool.Domain.Shared.Exceptions;
using System.Net;

namespace SeaQuail.DiagramTool.Host.AzureSwa.Infrastructure.AzureFuncApi.Default
{
    internal class FuncApiExceptionHandler : IFuncApiRequestHandler
    {
        private readonly IFuncApiRequestHandler wrapped;
        private readonly ILogger<FuncApiExceptionHandler> logger;

        public FuncApiExceptionHandler(IFuncApiRequestHandler wrapped, ILogger<FuncApiExceptionHandler> logger)
        {
            this.wrapped = wrapped;
            this.logger = logger;
        }

        public async Task<HttpResponseData> HandleRequest(HttpRequestData req)
        {
            try
            {
                return await wrapped.HandleRequest(req);
            }
            catch (Exception ex)
            {
                var status = HttpStatusCode.InternalServerError;
                if (ex is ServiceException svcEx)
                {
                    status = MapServiceException(svcEx);
                }
                logger.LogError(ex, "Caught exception for api call {apiMethod} {apiPath}", req.Method, req.Url.AbsolutePath);
                return req.CreateResponse(status);
            }
        }

        private HttpStatusCode MapServiceException(ServiceException ex)
            => ex.Reason switch
            {
                ServiceException.Reasons.NotFound => HttpStatusCode.NotFound,
                ServiceException.Reasons.NotPermitted => HttpStatusCode.Forbidden,
                ServiceException.Reasons.NoIdentity => HttpStatusCode.Unauthorized,
                ServiceException.Reasons.Internal => HttpStatusCode.ServiceUnavailable,
                ServiceException.Reasons.Unknown => HttpStatusCode.InternalServerError,
                ServiceException.Reasons.InvalidRequest => HttpStatusCode.BadRequest,
                _ => throw new NotImplementedException(),
            };
    }
}

using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.DependencyInjection;
using SeaQuail.DiagramTool.Domain.Shared.Exceptions;
using System.Net;
using System.Reflection;
using System.Text.Json;

namespace SeaQuail.DiagramTool.Host.AzureSwa.Infrastructure.AzureFuncApi.Default
{
    internal class AutoMethodInfoRequestHandlerBuilder : IRequestHandlerBuilder
    {
        public delegate Task<object?> ParameterAdapter(HttpRequestData req);
        public delegate Task<object?> ApiInvokeAdapter(HttpRequestData req);
        public delegate Task<HttpResponseData> ResponseAdapter(HttpRequestData req, object? apiResult);

        public RequestHandler CreateRequestAdapter(string path, MethodInfo apiMethod)
        {
            var invoke = CreateInvokeAdapter(apiMethod);
            var writeResponse = CreateResponseAdapter(apiMethod);

            return InvokeApi;

            async Task<HttpResponseData> InvokeApi(HttpRequestData req)
            {
                var result = await invoke(req);
                return await writeResponse(req, result);
            }
        }

        private ResponseAdapter CreateResponseAdapter(MethodInfo method)
        {
            var returnType = method.ReturnType;
            returnType = returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>)
                ? returnType.GenericTypeArguments[0]
                : returnType == typeof(Task)
                ? typeof(void)
                : null;

            ResponseAdapter handler = typeof(Stream).IsAssignableTo(returnType) ? HandleTaskStream
                : returnType == null ? HandleTaskVoid
                : HandleTaskData;

            return (req, returnValue)
                => TryCatch($"failed to read api response", () => handler(req, returnValue));

            Task<HttpResponseData> HandleTaskVoid(HttpRequestData req, object? returnValue)
            {
                return Task.FromResult(req.CreateResponse(HttpStatusCode.NoContent));
            }
            async Task<HttpResponseData> HandleTaskStream(HttpRequestData req, object? returnValue)
            {
                if (returnValue is Stream stream && stream != null)
                {
                    var result = req.CreateResponse(HttpStatusCode.OK);
                    await stream.CopyToAsync(result.Body);
                    return result;
                }
                return req.CreateResponse(HttpStatusCode.NoContent);
            }
            async Task<HttpResponseData> HandleTaskData(HttpRequestData req, object? returnValue)
            {
                var result = req.CreateResponse(HttpStatusCode.OK);
                await result.WriteAsJsonAsync(returnValue);
                return result;
            }
        }

        private ApiInvokeAdapter CreateInvokeAdapter(MethodInfo apiMethod)
        {
            var paramAdapters = apiMethod.GetParameters()
                .Select(CreateParameterAdapter)
                .ToArray();

            return (req) =>
                TryCatch("error invoking api", () => InvokeApi(req));

            async Task<object?> InvokeApi(HttpRequestData req)
            {
                var api = req.FunctionContext.InstanceServices.GetRequiredService(apiMethod.DeclaringType!);
                var parameters = new object?[paramAdapters.Length];
                foreach (var (paramAdapter, idx) in paramAdapters.Select((p, i) => (p, i)))
                {
                    parameters[idx] = await paramAdapter(req);
                }
                return apiMethod.Invoke(api, parameters);
            }
            ;
        }

        private ParameterAdapter CreateParameterAdapter(ParameterInfo paramInfo)
        {
            var adapter = CreateQueryAdapter(paramInfo) ?? CreateBodyAdapter(paramInfo);
            return (req) => TryCatch($"some prob w param '{paramInfo.Name}'", () => adapter(req));
        }

        private ParameterAdapter? CreateQueryAdapter(ParameterInfo paramInfo)
        {
            var paramName = paramInfo.Name;
            var paramType = paramInfo.ParameterType;
            var isNullableType = paramType.IsGenericType
                && paramType.GetGenericTypeDefinition() == typeof(Nullable<>);
            paramType = !isNullableType ? paramType : Nullable.GetUnderlyingType(paramType);
            var isGuidType = typeof(Guid) == paramType;
            var isQueryCompatible = typeof(IConvertible).IsAssignableFrom(paramType) || isGuidType;

            Func<string?, object?>? converter = !isQueryCompatible ? null
                : isGuidType ? ConvertGuid
                : paramType == typeof(string) ? ConvertStr
                : ConvertConvertible;

            return converter == null ? null
                : ((HttpRequestData req) =>
                {
                    var param = req.Query.Get(paramName);
                    return Task.FromResult<object?>(converter(param));
                });

            object? ConvertConvertible(string? value)
            {
                return value != null ? Convert.ChangeType(value, paramType!)
                    : !isNullableType ? null
                    : throw ServiceException.InvalidRequest($"'{paramName}' cant be empty");
            }
            object? ConvertStr(string? value) => value;
            object? ConvertGuid(string? value)
            {
                return Guid.TryParse(value?.ToString(), out var result) ? result
                    : throw ServiceException.InvalidRequest($"'{paramName}' supposed to be a guid");
            }
            object? MakeNullable(object? value)
            {
                return !isNullableType ? value
                    : value == null ? null
                    : Activator.CreateInstance(paramInfo.ParameterType, [value]);
            }
        }

        private ParameterAdapter CreateBodyAdapter(ParameterInfo paramInfo)
        {
            return paramInfo.ParameterType.IsAssignableTo(typeof(Stream))
                ? ((HttpRequestData req) => Task.FromResult<object?>(req.Body))
                : (async (HttpRequestData req) =>
                {
                    var result = await req.ReadAsStringAsync();
                    return result == null ? null : JsonSerializer.Deserialize(result, paramInfo.ParameterType);
                });
        }

        private async Task<T> TryCatch<T>(string message, Func<Task<T>> handler)
        {
            try
            {
                return await handler();
            }
            catch (Exception ex) when (ex is not ServiceException)
            {
                throw ServiceException.Unknown(message, ex);
            }
        }
    }
}

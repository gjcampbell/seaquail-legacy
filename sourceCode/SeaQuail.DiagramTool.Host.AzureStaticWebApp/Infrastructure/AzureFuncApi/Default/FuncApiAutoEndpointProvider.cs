using Microsoft.Azure.Functions.Worker.Http;
using SeaQuail.DiagramTool.Domain.Shared.Exceptions;
using System.Collections.Immutable;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SeaQuail.DiagramTool.Host.AzureSwa.Infrastructure.AzureFuncApi.Default;

internal class FuncApiAutoEndpointProvider : IFuncApiEndpointProvider
{
    internal record ApiPath(string Path, Type ApiType)
    {
        public ApiPath Repath(string path)
            => new ApiPath(path, ApiType);
    };
    
    private readonly static Regex apiNameParser = new(@"(?<name>[A-Z][a-zA-Z0-9]+)Api$");
    private readonly static Regex apiPathPartParser = new(@"^[A-Z][a-zA-Z0-9]+$");

    private readonly Lazy<IReadOnlyCollection<(string path, RequestHandler handler)>> handlers;
    
    public FuncApiAutoEndpointProvider(IEnumerable<ApiPath> paths, IRequestHandlerBuilder methodAdapter)
    {
        var apiPaths = paths.ToImmutableList();
        handlers = new(() => CreateHandlers(methodAdapter, apiPaths));
    }

    public IEnumerable<(string path, RequestHandler handler)> GetHandlers()
        => handlers.Value;

    private IReadOnlyCollection<(string path, RequestHandler handler)> CreateHandlers(IRequestHandlerBuilder methodAdapter, IEnumerable<ApiPath> apiPaths)
    {
        var endpoints = DetectEndpoints(apiPaths);
        var handlers = endpoints.Select((item) => (item.path, methodAdapter.CreateRequestAdapter(item.path, item.method))).ToImmutableList();
        return handlers;
    }

    private (string path, MethodInfo method)[] DetectEndpoints(IEnumerable<ApiPath> apiPaths)
        => apiPaths
            .Select(p => (lcPath: p.Path.Trim('/').ToLower(), type: p.ApiType))
            .SelectMany(p => GetCandidateMethods(p.type).Where(IsValidMethod).Select(m => GetEndpoint(p.lcPath, m)))
            .ToArray();

    private IEnumerable<MethodInfo> GetCandidateMethods(Type type)
        => type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.InvokeMethod);

    private bool IsValidMethod(MethodInfo method)
        => typeof(Task).IsAssignableFrom(method.ReturnType)
            && apiPathPartParser.IsMatch(method.Name);

    private (string path, MethodInfo method) GetEndpoint(string typePathLc, MethodInfo method)
        => ($"{typePathLc}/{method.Name.ToLower()}", method);

    public static ApiPath CreateApiPath<T>(string? explicitPath)
        => new ApiPath(
            explicitPath
            ?? apiNameParser.Match(typeof(T).Name).Groups["name"]?.Value
            ?? throw ServiceException.NotFound(),
            typeof(T)
            );
}

interface IRequestHandlerBuilder
{
    RequestHandler CreateRequestAdapter(string path, MethodInfo apiMethod);
}

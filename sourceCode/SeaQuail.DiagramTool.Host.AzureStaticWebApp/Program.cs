using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SeaQuail.DiagramTool.Infrastructure.AzureTable;
using SeaQuail.DiagramTool.Domain.Services;
using Microsoft.Extensions.Configuration;
using SeaQuail.DiagramTool.Host.AzureSwa.Infrastructure.AzureFuncApi;
using SeaQuail.DiagramTool.Domain.Apis;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();


builder.Services
    .AddLogging()
    .AddAzureTablePersistence(builder.Configuration.GetValue<string>("AzureStorage")!)
    .AddTransient<DiagramService>()
    .AddTransient<CurrentUserService>()
    .AddDefaultFuncApis("api")
    .AddApi<DiagramApi>()
    ;


builder.Build().Run();

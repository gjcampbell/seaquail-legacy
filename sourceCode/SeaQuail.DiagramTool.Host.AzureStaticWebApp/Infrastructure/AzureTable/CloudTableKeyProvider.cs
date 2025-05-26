using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SeaQuail.DiagramTool.Domain.Shared.Services;
using System;

namespace SeaQuail.DiagramTool.Infrastructure.AzureTable
{
    public class CloudTableKeyProvider
    {
        public (string partitionKey, string rowKey) GetKey(Guid id, Type type)
        {
            var rowKey = id.ToString();
            var partition = "1";
            return (partition, rowKey);
        }
    }

    public static class Extensions
    {
        public static IServiceCollection AddAzureTablePersistence(this IServiceCollection services, string azureStorageConn)
        {
            return services
                .AddSingleton(p => new TableServiceClient(azureStorageConn))
                .AddSingleton<CloudTableKeyProvider>()
                .AddSingleton(typeof(IPersistenceService<>), typeof(CloudTablePersistenceService<>));
        }
    }
}

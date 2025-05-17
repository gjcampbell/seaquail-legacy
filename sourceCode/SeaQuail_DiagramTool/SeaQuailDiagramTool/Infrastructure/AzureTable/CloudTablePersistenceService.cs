using Azure.Data.Tables;
using Mapster;
using SeaQuailDiagramTool.Domain.Models;
using SeaQuailDiagramTool.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SeaQuailDiagramTool.Infrastructure
{
    public class CloudTablePersistenceService<T> : IPersistenceService<T> where T : IHaveID
    {
        private readonly TableClient table;
        private readonly CloudTableKeyProvider keyProvider;

        public CloudTablePersistenceService(TableServiceClient tableServiceClient, CloudTableKeyProvider keyProvider)
        {
            table = tableServiceClient.GetTableClient(typeof(T).Name);
            this.keyProvider = keyProvider;
        }

        public async Task Delete(Guid id)
        {
            var (partitionKey, rowKey) = keyProvider.GetKey(id, typeof(T));
            await table.DeleteEntityAsync(partitionKey, rowKey);
        }


        public async Task<IEnumerable<T>> Filter(Expression<Func<T, bool>> criteria)
        {
            var exprAdapter = new TableEntityExpressionAdapter();
            var query = exprAdapter.Adapt(criteria);
            var response = table.QueryAsync<TableEntity>(query);
            var results = await response.ToEnumerable();
            return results.Select(r => r.Adapt<T>());
        }

        public async Task<T> GetById(Guid id)
        {
            var (partitionKey, rowKey) = keyProvider.GetKey(id, typeof(T));
            var response = await table.GetEntityAsync<TableEntity>(partitionKey, rowKey);
            return response.Value.Adapt<T>();
        }

        public async Task<T> Save(T item)
        {
            if (item.Id == Guid.Empty)
            {
                item.Id = Guid.NewGuid();
                item.CreateDate = DateTimeOffset.UtcNow;
            }
            item.ModifyDate = DateTimeOffset.UtcNow;
            var (partitionKey, rowKey) = keyProvider.GetKey(item.Id, typeof(T));

            var entity = item.Adapt<TableEntity>();
            entity.RowKey = rowKey;
            entity.PartitionKey = partitionKey;
            
            await table.UpsertEntityAsync(entity);
            return item;
        }
    }
}

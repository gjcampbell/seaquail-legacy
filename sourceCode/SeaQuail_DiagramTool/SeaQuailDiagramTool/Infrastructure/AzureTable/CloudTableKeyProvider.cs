using System;

namespace SeaQuailDiagramTool.Infrastructure
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
}

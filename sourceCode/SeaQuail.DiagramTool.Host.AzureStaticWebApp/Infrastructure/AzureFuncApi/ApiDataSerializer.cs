using System.Text.Json;
using System.Text.Json.Serialization;

namespace SeaQuail.DiagramTool.Host.AzureSwa.Infrastructure.AzureFuncApi
{
    internal class ApiDataSerializer
    {
        private readonly JsonSerializerOptions _serOptions;

        public ApiDataSerializer()
        {
            _serOptions = new()
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
            };
            _serOptions.Converters.Add(new JsonStringEnumConverter());
        }

        public string Serialize<T>(T item)
            => JsonSerializer.Serialize(item);

        public T? Deserialize<T>(string json)
            => JsonSerializer.Deserialize<T>(json);
    }
}

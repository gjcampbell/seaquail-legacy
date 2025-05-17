using System.Text.Json;

namespace SeaQuailDiagramTool.Application
{
    public class DiagramSerializer
    {
        public DiagramSerializer()
        {
        }

        public string Serialize<T>(T item)
            => JsonSerializer.Serialize(item);

        public T Deserialize<T>(string json)
            => JsonSerializer.Deserialize<T>(json);
    }
}

using System;

namespace SeaQuailDiagramTool.Domain.Models
{
    public class DiagramSnapshot : BaseModel
    {
        public string Name { get; set; }
        public Guid DiagramId { get; set; }
        public bool IsDefault { get; set; }
        public string DiagramData { get; set; }
    }
}

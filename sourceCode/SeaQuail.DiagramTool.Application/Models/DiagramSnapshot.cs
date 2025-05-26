using System;

namespace SeaQuail.DiagramTool.Domain.Models
{
    public class DiagramSnapshot : BaseModel
    {
        public string Name { get; set; }
        public Guid DiagramId { get; set; }
        public bool IsDefault { get; set; }
        public string DiagramData { get; set; }
    }
}

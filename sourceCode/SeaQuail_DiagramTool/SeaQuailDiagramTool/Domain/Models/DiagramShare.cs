using System;

namespace SeaQuailDiagramTool.Domain.Models
{
    public enum DiagramSharePermission { View, Edit }
    public class DiagramShare : BaseModel
    {
        public Guid Id { get; set; }
        public Guid DiagramId { get; set; }
        public DiagramSharePermission Permission { get; set; }
        public string Email { get; set; }
    }
}

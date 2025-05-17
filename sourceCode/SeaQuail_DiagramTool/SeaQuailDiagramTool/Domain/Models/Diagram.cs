using System;
using System.Collections.Generic;

namespace SeaQuailDiagramTool.Domain.Models
{
    public class Diagram : BaseModel
    {
        public IList<DiagramSnapshot> Snapshots { get; set; }
        public string Name { get; set; }
        public Guid OwnerId { get; set; }
        public bool AllowPublicAccess { get; set; }
    }
}

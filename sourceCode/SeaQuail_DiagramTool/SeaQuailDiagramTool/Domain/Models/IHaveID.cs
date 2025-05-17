using System;

namespace SeaQuailDiagramTool.Domain.Models
{
    public interface IHaveID
    {
        Guid Id { get; set; }
        DateTimeOffset CreateDate { get; set; }
        DateTimeOffset ModifyDate { get; set; }
    }
}

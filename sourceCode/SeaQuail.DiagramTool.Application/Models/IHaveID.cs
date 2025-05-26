using System;

namespace SeaQuail.DiagramTool.Domain.Models
{
    public interface IHaveID
    {
        Guid Id { get; set; }
        DateTimeOffset CreateDate { get; set; }
        DateTimeOffset ModifyDate { get; set; }
    }
}

using System;

namespace SeaQuail.DiagramTool.Domain.Models
{
    public abstract class BaseModel : IHaveID
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreateDate {get;set;}
        public DateTimeOffset ModifyDate {get;set;}
    }
}

namespace SeaQuailDiagramTool.Domain.Models
{
    public class User : BaseModel
    {
        public string ExternalId { get; set; }
        public string Email { get; set; }
    }
}

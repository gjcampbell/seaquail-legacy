namespace SeaQuailDiagramTool.Domain.Services
{
    public interface IUserProvider
    {
        (string email, string externalId)? GetCurrentUser();
    }
}

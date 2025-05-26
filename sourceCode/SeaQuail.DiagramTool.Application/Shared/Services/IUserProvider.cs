namespace SeaQuail.DiagramTool.Domain.Shared.Services
{
    public interface IUserProvider
    {
        (string email, string externalId)? GetCurrentUser();
    }
}

using SeaQuail.DiagramTool.Domain.Models;
using SeaQuail.DiagramTool.Domain.Shared.Exceptions;
using SeaQuail.DiagramTool.Domain.Shared.Services;
using System.Threading.Tasks;

namespace SeaQuail.DiagramTool.Domain.Services
{
    public class CurrentUserService
    {
        private readonly IUserProvider userProvider;
        private readonly IPersistenceService<User> userPersistence;

        public CurrentUserService(IUserProvider userProvider,
            IPersistenceService<User> userPersistence)
        {
            this.userProvider = userProvider;
            this.userPersistence = userPersistence;
        }

        public string? GetUserExternalId()
            => userProvider.GetCurrentUser()?.externalId;
        public async Task<User> GetUser()
        {
            var externalId = GetUserExternalId();
            if (externalId == null)
                throw ServiceException.Internal("no current user");

            return await userPersistence.GetOne(u => u.ExternalId == externalId);
        }

        public async Task CreateIfNotExists()
        {
            var (email, externalId) = userProvider.GetCurrentUser() ?? default;
            var result = await userPersistence.GetOne(u => u.ExternalId == externalId);
            if (result == null)
            {
                result = new() { Email = email, ExternalId = externalId };
                await userPersistence.Save(result);
            }
        }
    }
}

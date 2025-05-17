using SeaQuailDiagramTool.Domain.Models;
using System.Threading.Tasks;

namespace SeaQuailDiagramTool.Domain.Services
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

        public async Task<User> GetUser()
        {
            var externalId = userProvider.GetCurrentUser()?.externalId;
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

using Microsoft.AspNetCore.Http;
using SeaQuailDiagramTool.Domain.Services;
using System.Security.Claims;

namespace SeaQuailDiagramTool.Application
{
    public class HttpUserProvider : IUserProvider
    {
        private readonly IHttpContextAccessor contextAccessor;

        public HttpUserProvider(IHttpContextAccessor contextAccessor)
        {
            this.contextAccessor = contextAccessor;
        }

        public (string email, string externalId)? GetCurrentUser()
        {
            var user = contextAccessor.HttpContext.User;
            if (user.Identity.IsAuthenticated)
            {
                return (user.FindFirstValue(ClaimTypes.Email), user.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier"));
            }
            return null;
        }
    }
}

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using SeaQuailDiagramTool.Domain;
using SeaQuailDiagramTool.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeaQuailDiagramTool.Application
{
    public class AuthController : Controller
    {
        private readonly CurrentUserService currentUserService;

        public AuthController(CurrentUserService currentUserService)
        {
            this.currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task SignIn()
        {
            await Request.HttpContext.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, new()
            {
                RedirectUri = "/Auth/EstablishUser"
            });
        }

        [HttpGet]
        new public async Task SignOut()
        {
            await Request.HttpContext.SignOutAsync();
        }

        [HttpGet]
        public async Task<IActionResult> EstablishUser()
        {
            await currentUserService.CreateIfNotExists();
            return Redirect("/close");
        }
    }
}

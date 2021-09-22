using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CognitoDotNet.WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        public IActionResult LogOut()
        {
            return SignOut(new string[] { 
                CookieAuthenticationDefaults.AuthenticationScheme, 
                OpenIdConnectDefaults.AuthenticationScheme 
            });
        }

        public IActionResult LoggedOut()
        {
            return View();
        }
    }
}
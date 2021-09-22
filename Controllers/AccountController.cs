using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AWSLambdaCognitoOpenIDConnect.Controllers
{
    public class AccountController : Controller
    {
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
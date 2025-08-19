using App.Common.Infrastructure.Cache;
using App.Web.UI.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Web.UI.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login() => View();

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Unauthorized() => View();

        [HttpGet]
        public IActionResult AccessDenied() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoutAsync()
        {
            var userId = User.GetIdentifier();
            var sessionId = User.GetSessionId();
            var sessionKey = $"session-{userId}";
            var cache = HttpContext.RequestServices.GetRequiredService<ICacheService>();

            await cache.RemoveAsync(sessionKey);

            return SignOut(
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("Login", "Account") // after logout
                },
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        // [HttpGet]
        // [AllowAnonymous]
        // public IActionResult Login(string returnUrl = null)
        // {
        //     var properties = new AuthenticationProperties
        //     {
        //         RedirectUri = returnUrl ?? Url.Action("Index", "Home")
        //     };

        //     // Challenge with OpenIdConnect scheme to trigger Azure AD login
        //     return Challenge(properties, OpenIdConnectDefaults.AuthenticationScheme);
        // }

        // [HttpGet]
        // [AllowAnonymous]
        // public async Task<IActionResult> LoginCallback()
        // {
        //     // Get the login info + user
        //     var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        //     if (!authenticateResult.Succeeded)
        //     {
        //         return RedirectToAction(nameof(Login));
        //     }

        //     // User is now logged in, redirect to home
        //     return RedirectToAction("Index", "Home");
        // }
    }
}

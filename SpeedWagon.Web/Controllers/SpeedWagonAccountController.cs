using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using SpeedWagon.Web.Enum;
using SpeedWagon.Web.Interfaces;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SpeedWagon.Web.Controllers
{

    public class SpeedWagonAccountController : Controller
    {

        private readonly IAuthTypeInformationProvider _authTypeInformationProvider;

        public SpeedWagonAccountController(IAuthTypeInformationProvider authTypeInformationProvider)
        {
            this._authTypeInformationProvider = authTypeInformationProvider;
        }


        [HttpGet]
        public async Task<IActionResult> LogIn()
        {

            if (this._authTypeInformationProvider.GetAuthType() == AuthType.AzureAd)
            {
                return Challenge(
                new AuthenticationProperties { RedirectUri = "/" },
                OpenIdConnectDefaults.AuthenticationScheme);
            }

            ClaimsIdentity identity = new ClaimsIdentity(GetUserRoleClaims(), CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Redirect("/SpeedWagon");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()      
        {
            if (this._authTypeInformationProvider.GetAuthType() == AuthType.AzureAd)
            {
                return SignOut(
                    new AuthenticationProperties { RedirectUri = "/" },
                    CookieAuthenticationDefaults.AuthenticationScheme
                    , OpenIdConnectDefaults.AuthenticationScheme);
            } 

            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

        private IEnumerable<Claim> GetUserRoleClaims()
        {
            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, "testuser"));
            claims.Add(new Claim(ClaimTypes.Name, "testuser"));
            claims.Add(new Claim(ClaimTypes.Role, "user"));
            return claims;
        }

    }
}
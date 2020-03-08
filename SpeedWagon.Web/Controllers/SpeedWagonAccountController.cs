using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SpeedWagon.Web.Controllers
{

    public class SpeedWagonAccountController : Controller
    {
        
        public SpeedWagonAccountController()
        {
           
        }


        [HttpGet]
        public async Task<IActionResult> LogIn()
        {

            ClaimsIdentity identity = new ClaimsIdentity(GetUserRoleClaims(), CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Redirect("/SpeedWagon");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

        private IEnumerable<Claim> GetUserRoleClaims()
        {
            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, "tesuser"));
            claims.Add(new Claim(ClaimTypes.Name, "tesuser"));
            claims.Add(new Claim(ClaimTypes.Role, "user"));
            return claims;
        }

    }
}

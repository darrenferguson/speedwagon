using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using SpeedWagon.Web.Interfaces;

namespace SpeedWagon.Web.Controllers
{
    [Authorize]
    public class SpeedWagonController : Controller
    {
        private readonly ISpeedWagonAdminContext _speedWagon;
        private readonly ICompositeViewEngine _viewEngine;

        public SpeedWagonController(ISpeedWagonAdminContext speedWagon, ICompositeViewEngine viewEngine)
        {
            this._speedWagon = speedWagon;
            this._viewEngine = viewEngine;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Install()
        {
            string path = this._speedWagon.Install(User.Identity.Name);
            return View();
        }
    }
}
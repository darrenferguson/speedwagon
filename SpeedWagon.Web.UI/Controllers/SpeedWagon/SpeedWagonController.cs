using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpeedWagon.Web.Interfaces;

namespace SpeedWagon.Web.UI.Controllers
{
    [Authorize]
    public class SpeedWagonController : Controller
    {
        private readonly ISpeedWagonWebContext _speedWagon;

        public SpeedWagonController(ISpeedWagonWebContext speedWagon)
        {
            this._speedWagon = speedWagon;
        }

        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Install()
        {
            string path = this._speedWagon.Install();

            return View();
        }
    }
}

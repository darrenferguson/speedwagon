using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpeedWagon.Models;
using SpeedWagon.Web.Interfaces;
using SpeedWagon.Web.Models;
using SpeedWagon.Web.UI.Models;

namespace SpeedWagon.Web.UI.Controllers
{

    public class HomeController : Controller
    {
        private readonly ISpeedWagonWebContext _speedWagon;

        public HomeController(ISpeedWagonWebContext speedWagon)
        {
            this._speedWagon = speedWagon;
        }

        public IActionResult Index()
        {
            SpeedWagonPage model = this._speedWagon.PageFor(Request);           
            return View(model);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

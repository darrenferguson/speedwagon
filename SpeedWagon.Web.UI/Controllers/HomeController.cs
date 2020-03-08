using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpeedWagon.Models;
using SpeedWagon.Web.Extension;
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

        public async Task<IActionResult> Index()
        {
            SpeedWagonPage model = await this._speedWagon.PageFor(Request);           
            return View(model.Content.View(), model);
        }

        

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

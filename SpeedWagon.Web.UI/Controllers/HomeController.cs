using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpeedWagon.Models;
using SpeedWagon.Runtime.Interfaces;
using SpeedWagon.Web.Extension;
using SpeedWagon.Web.Interfaces;
using SpeedWagon.Web.Models;
using SpeedWagon.Web.UI.Models;
using SpeedWagon.Web.UI.Models.Page;

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
            if(model.Status == 404)
            {
                return View("~/Views/404.cshtml", model);
            }

            if(model.Content.Type == "Home")
            {
                return await Home(model);
            }

            return View(model.Content.View(), model);
        }

        public async Task<IActionResult> Home(SpeedWagonPage model)
        {
            HomePage page = new HomePage(model);
            page.Posts = await this._speedWagon.SearchService.Search(new Dictionary<string, string>() {{ "Type", "Post" }});
            return View(page.Content.View(), page);
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

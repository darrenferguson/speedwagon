using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Newtonsoft.Json.Linq;
using SpeedWagon.Models;
using SpeedWagon.Runtime.Extension;
using SpeedWagon.Web.Helper;
using SpeedWagon.Web.Interfaces;
using SpeedWagon.Web.Models.ContentType;
using SpeedWagon.Web.Models.View.Content;
using SpeedWagon.Web.Models.View.Editor;
using System.Collections.Generic;
using System.Linq;

namespace SpeedWagon.Web.UI.Controllers
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
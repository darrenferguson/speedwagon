using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpeedWagon.Models;
using SpeedWagon.Web.Interfaces;

namespace SpeedWagon.Web.UI.Controllers
{
    public class SpeedWagonContentController : Controller
    {
        private readonly ISpeedWagonWebContext _speedWagon;
        protected readonly SpeedWagonContent _content;

        public SpeedWagonContentController(ISpeedWagonWebContext speedWagon)
        {
            this._speedWagon = speedWagon;
            
        }

    }
}

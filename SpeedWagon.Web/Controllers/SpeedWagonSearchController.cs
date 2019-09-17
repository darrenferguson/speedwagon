using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpeedWagon.Runtime.Interfaces;
using SpeedWagon.Web.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SpeedWagon.Web.Models.View.Search;

namespace SpeedWagon.Web.Controllers
{
    public class SpeedWagonSearchController : Controller
    {
        private readonly ISpeedWagonAdminContext _speedwagon;

        public SpeedWagonSearchController(ISpeedWagonAdminContext speedwagon)
        {
            this._speedwagon = speedwagon;
        }

        public IActionResult Index()
        {
            return View("~/Views/SpeedWagon/Search/Index.cshtml", new SearchViewModel());
        }

        public async Task<IActionResult> IndexAll()
        {
            await this._speedwagon.SearchService.IndexAll(this._speedwagon.ContentService);

            return Redirect("SpeedWagonSearch");
        }

        [HttpPost]
        public async Task<IActionResult> Search(SearchViewModel viewModel)
        {
            var results = await this._speedwagon.SearchService.Search(viewModel.Term);

            viewModel.Results = results;
            return View("~/Views/SpeedWagon/Search/Index.cshtml", viewModel);
        }
    }
}
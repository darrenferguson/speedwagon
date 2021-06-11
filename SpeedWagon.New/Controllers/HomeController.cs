using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpeedWagon.Web.Extension;
using SpeedWagon.Web.Interfaces;
using SpeedWagon.Web.Models;
using SpeedWagon.New.Models;
using SpeedWagon.New.Models.Page;

namespace SpeedWagon.New.Controllers
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

            Site site = new Site(model);
            site.Home = await this._speedWagon.ContentFor(Request, "/");
            site.TopNavigation = await this._speedWagon.ContentService.Children(site.Home);
            
            if (model.Status == 404)
            {
                return View("~/Views/404.cshtml", site);
            }
 
            if (model.Content.Type == "Home")
            {
                return await Home(site);
            } 
            else if (model.Content.Type == "Rssfeed")
            {
                return await Rss(site);
            }

            return View(model.Content.View(), site);
        }

        public async Task<IActionResult> Rss(Site site)
        {
            Response.ContentType = "application/rss+xml";
            RssFeed rssFeed = new RssFeed(site);
            rssFeed.Posts = await this._speedWagon.SearchService.Search(new Dictionary<string, string>() { { "Type", "Post" } });
            return View(rssFeed.Content.View(), rssFeed);
        }

        public async Task<IActionResult> Home(Site site)
        {
            HomePage homePage = new HomePage(site);

            homePage.Posts = await this._speedWagon.SearchService.Search(new Dictionary<string, string>() {{ "Type", "Post" }});
            homePage.Posts = homePage.Posts.OrderByDescending(x => x.Content.GetValue<DateTime>("Date"));
            return View(homePage.Content.View(), homePage);
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SpeedWagon.Models;
using SpeedWagon.Web.Helper;
using SpeedWagon.Web.Interfaces;
using SpeedWagon.Web.Models.View.Content;
using System.Collections.Generic;

namespace SpeedWagon.Web.Controllers
{
    [Authorize]
    public class SpeedWagonContentController : Controller
    {
        private readonly ISpeedWagonAdminContext _speedWagon;

        public SpeedWagonContentController(ISpeedWagonAdminContext speedWagon)
        {
            this._speedWagon = speedWagon;
        }

        public IActionResult List(string url = null)
        {
            SpeedWagonContent contentRoot = this._speedWagon.WebContentService.GetContent(url);
            IEnumerable<SpeedWagonContent> contents = this._speedWagon.ContentService.Children(contentRoot);

            IEnumerable<SpeedWagonContent> contentTypes = this._speedWagon.ContentTypeService.List();
            IList<SelectListItem> contentTypeSelct = SelectListHelper.GetSelectList(contentTypes);

            ContentViewModel viewModel = new ContentViewModel();
            viewModel.AvailableContentTypes = contentTypeSelct;
            viewModel.Content = contentRoot;
            viewModel.Contents = contents;
            viewModel.ContentService = this._speedWagon.ContentService;

            return View("~/Views/SpeedWagon/Content/List.cshtml", viewModel);
        }


        [HttpPost]
        public IActionResult Add(ContentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                SpeedWagonContent contentRoot = this._speedWagon.WebContentService.GetContent(viewModel.Parent);
                IEnumerable<SpeedWagonContent> contents = this._speedWagon.ContentService.Children(contentRoot);

                IEnumerable<SpeedWagonContent> contentTypes = this._speedWagon.ContentTypeService.List();
                IList<SelectListItem> contentTypeSelct = SelectListHelper.GetSelectList(contentTypes);

                viewModel.AvailableContentTypes = contentTypeSelct;
                viewModel.Content = contentRoot;
                viewModel.Contents = contents;
                viewModel.ContentService = this._speedWagon.ContentService;

                return View(viewModel);
            }

            this._speedWagon.WebContentService.Add(viewModel.Parent, viewModel.Name, viewModel.Type, User.Identity.Name);
            return RedirectToAction("Content", new { url = viewModel.Parent });
        }
    }
}

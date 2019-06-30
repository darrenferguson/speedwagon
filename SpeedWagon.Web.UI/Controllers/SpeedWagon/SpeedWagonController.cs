using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpeedWagon.Models;
using SpeedWagon.Web.Interfaces;
using SpeedWagon.Web.Models.View.Editor;
using System.Collections.Generic;

namespace SpeedWagon.Web.UI.Controllers
{
    [Authorize]
    public class SpeedWagonController : Controller
    {
        private readonly ISpeedWagonAdminContext _speedWagon;

        public SpeedWagonController(ISpeedWagonAdminContext speedWagon)
        {
            this._speedWagon = speedWagon;
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

        public IActionResult Content()
        {
            return View();
        }

        public IActionResult Editor()
        {
            SpeedWagonContent editorRoot = this._speedWagon.GetContent("/editors");
            IEnumerable<SpeedWagonContent> editors = this._speedWagon.ContentService.Children(editorRoot);

            EditorViewModel viewModel = new EditorViewModel();
            viewModel.Editors = editors;

            return View(viewModel);
        }


        [HttpPost]
        public IActionResult Editor(EditorViewModel model)
        {

            if (!ModelState.IsValid)
            {
                SpeedWagonContent editorRoot = this._speedWagon.GetContent("/editors");
                IEnumerable<SpeedWagonContent> editors = this._speedWagon.ContentService.Children(editorRoot);

                model.Editors = editors;

                return View(model);
            }

            this._speedWagon.AddEditor(model.Name, User.Identity.Name);

            return RedirectToAction("Editor");
        }


        public IActionResult ContentType()
        {
            SpeedWagonContent editorRoot = this._speedWagon.GetContent("/editors");
            IEnumerable<SpeedWagonContent> editors = this._speedWagon.ContentService.Children(editorRoot);

            SpeedWagonContent contentTypeRoot = this._speedWagon.GetContent("/content-types");
            IEnumerable<SpeedWagonContent> contentTypes = this._speedWagon.ContentService.Children(contentTypeRoot);


            ContentTypeViewModel viewModel = new ContentTypeViewModel();
            viewModel.Editors = editors;
            viewModel.ContentTypes = contentTypes;

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult ContentType(ContentTypeViewModel model)
        {

            if (!ModelState.IsValid)
            {
                SpeedWagonContent editorRoot = this._speedWagon.GetContent("/editors");
                IEnumerable<SpeedWagonContent> editors = this._speedWagon.ContentService.Children(editorRoot);

                SpeedWagonContent contentTypeRoot = this._speedWagon.GetContent("/content-types");
                IEnumerable<SpeedWagonContent> contentTypes = this._speedWagon.ContentService.Children(contentTypeRoot);


                model.Editors = editors;
                model.ContentTypes = contentTypes;

                return View(model);
            }

            this._speedWagon.AddContentType(model.Name, User.Identity.Name);

            return RedirectToAction("ContentType");
        }

        public IActionResult EditContentType(string id)
        {
            SpeedWagonContent editorRoot = this._speedWagon.GetContent("/editors");
            IEnumerable<SpeedWagonContent> editors = this._speedWagon.ContentService.Children(editorRoot);

            SpeedWagonContent contentTypeRoot = this._speedWagon.GetContent("/content-types");
            IEnumerable<SpeedWagonContent> contentTypes = this._speedWagon.ContentService.Children(contentTypeRoot);


            ContentTypeViewModel viewModel = new ContentTypeViewModel();
            viewModel.Editors = editors;
            viewModel.ContentTypes = contentTypes;

            return View(viewModel);
        }
    }
}

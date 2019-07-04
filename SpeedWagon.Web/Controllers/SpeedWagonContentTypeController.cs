using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpeedWagon.Models;
using SpeedWagon.Web.Helper;
using SpeedWagon.Web.Interfaces;
using SpeedWagon.Web.Models.View.Editor;
using System;
using System.Collections.Generic;

namespace SpeedWagon.Web.Controllers
{
    [Authorize]
    public class SpeedWagonContentTypeController : Controller
    {
        private readonly ISpeedWagonAdminContext _speedWagon;

        public SpeedWagonContentTypeController(ISpeedWagonAdminContext speedWagon)
        {
            this._speedWagon = speedWagon;
        }

        public IActionResult List()
        {
            ContentTypeViewModel viewModel = new ContentTypeViewModel();
            viewModel.ContentTypes = this._speedWagon.ContentTypeService.List();

            return View("~/Views/SpeedWagon/ContentType/List.cshtml", viewModel);
        }

        [HttpPost]
        public IActionResult Add(ContentTypeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ContentTypes = this._speedWagon.ContentTypeService.List();
                return View("~/Views/SpeedWagon/ContentType/List.cshtml", model);
            }

            this._speedWagon.ContentTypeService.Add(model.Name, User.Identity.Name);
            return RedirectToAction("List", new { id = model.Name });
        }

        public IActionResult Edit(string id)
        {
            SpeedWagonContent contentType = this._speedWagon.ContentTypeService.Get(id);
            IEnumerable<SpeedWagonContent> editors = this._speedWagon.EditorService.List();

            EditContentTypeViewModel viewModel = new EditContentTypeViewModel();

            viewModel.ContentType = contentType;
            viewModel.Name = contentType.Name;
            viewModel.AvailableEditors = SelectListHelper.GetSelectList(editors);
            viewModel.Editors = this._speedWagon.ContentTypeService.GetEditors(contentType);

            return View("~/Views/SpeedWagon/ContentType/Edit.cshtml", viewModel);
        }

        [HttpPost]
        public IActionResult Edit(EditContentTypeViewModel viewModel)
        {   
            SpeedWagonContent contentType = this._speedWagon.ContentTypeService.Get(viewModel.Name);

            if (!ModelState.IsValid)
            {
                IEnumerable<SpeedWagonContent> editors = this._speedWagon.EditorService.List();

                viewModel.ContentType = contentType;
                viewModel.Name = contentType.Name;

                viewModel.AvailableEditors = SelectListHelper.GetSelectList(editors);
                viewModel.Editors = this._speedWagon.ContentTypeService.GetEditors(contentType);

                return View(viewModel);
            }

            this._speedWagon.ContentTypeService.AddEditor(contentType, viewModel.ContentTypeEditor);
            this._speedWagon.ContentTypeService.Save(contentType, User.Identity.Name);

            return RedirectToAction("Edit", new { id = viewModel.Name });
        }

        [HttpPost]
        public IActionResult Delete(DeleteEditorModel model)
        {
            throw new NotImplementedException();
        }
    }
}

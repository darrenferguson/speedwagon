using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpeedWagon.Models;
using SpeedWagon.Web.Helper;
using SpeedWagon.Web.Interfaces;
using SpeedWagon.Web.Models.ContentType;
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
            IEnumerable<SpeedWagonContent> contentTypes = this._speedWagon.ContentTypeService.List();

            viewModel.AvailableContentTypes = SelectListHelper.GetSelectList(contentTypes);

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

            this._speedWagon.ContentTypeService.Add(model.Name, User.Identity.Name, model.Root, model.Children);
            return RedirectToAction("List", new { id = model.Name });
        }

        public IActionResult Edit(string name)
        {
            SpeedWagonContent contentType = this._speedWagon.ContentTypeService.Get(name);
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

            return RedirectToAction("Edit", new { name = viewModel.Name });
        }

        [HttpPost]
        public IActionResult Delete(DeleteEditorModel model)
        {
            this._speedWagon.ContentTypeService.Delete(model.Name);
            return RedirectToAction("List");
        }

        [HttpPost]
        public IActionResult MoveEditorUp(ContentTypeEditor model)
        {
            SpeedWagonContent contentType = this._speedWagon.ContentTypeService.Get(model.Name);
            this._speedWagon.ContentTypeService.MoveEditorUp(contentType, model.Editor);
            this._speedWagon.ContentTypeService.Save(contentType, User.Identity.Name);

            return RedirectToAction("Edit", new { name = model.Name });
        }

        [HttpPost]
        public IActionResult MoveEditorDown(ContentTypeEditor model)
        {
            SpeedWagonContent contentType = this._speedWagon.ContentTypeService.Get(model.Name);
            this._speedWagon.ContentTypeService.MoveEditorDown(contentType, model.Editor);
            this._speedWagon.ContentTypeService.Save(contentType, User.Identity.Name);

            return RedirectToAction("Edit", new { name = model.Name });
        }

        [HttpPost]
        public IActionResult DeleteEditor(ContentTypeEditor model)
        {
            SpeedWagonContent contentType = this._speedWagon.ContentTypeService.Get(model.Name);
            this._speedWagon.ContentTypeService.DeleteEditor(contentType, model.Editor);
            this._speedWagon.ContentTypeService.Save(contentType, User.Identity.Name);

            return RedirectToAction("Edit", new { name = model.Name });
        }
    }
}

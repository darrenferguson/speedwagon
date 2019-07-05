using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpeedWagon.Models;
using SpeedWagon.Web.Extension;
using SpeedWagon.Web.Helper;
using SpeedWagon.Web.Interfaces;
using SpeedWagon.Web.Models.ContentType;
using SpeedWagon.Web.Models.View.Editor;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public IActionResult Edit(string url, string operation = null)
        {
            SpeedWagonContent contentType = this._speedWagon.ContentTypeService.Get(url);
            IEnumerable<SpeedWagonContent> editors = this._speedWagon.EditorService.List();
            IEnumerable<SpeedWagonContent> contentTypes = this._speedWagon.ContentTypeService.List();

            EditContentTypeViewModel viewModel = new EditContentTypeViewModel();

            viewModel.Operation = operation;
            viewModel.ContentType = contentType;
            viewModel.Root = contentType.GetValue<bool>("Root");
            viewModel.Children = contentType.GetValue<string[]>("Children");
            viewModel.Name = contentType.Name;
            viewModel.Url = url;

            viewModel.AvailableContentTypes = SelectListHelper.GetSelectList(contentTypes);
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
                IEnumerable<SpeedWagonContent> contentTypes = this._speedWagon.ContentTypeService.List();

                viewModel.ContentType = contentType;
                viewModel.Root = contentType.GetValue<bool>("Root");
                viewModel.Children = contentType.GetValue<string[]>("Children");
                viewModel.Name = contentType.Name;
                viewModel.Url = contentType.RelativeUrl;

                viewModel.AvailableContentTypes = SelectListHelper.GetSelectList(contentTypes);
                viewModel.AvailableEditors = SelectListHelper.GetSelectList(editors);
                viewModel.Editors = this._speedWagon.ContentTypeService.GetEditors(contentType);

                return View(viewModel);
            }


            contentType.Content["Root"] = viewModel.Root;
            contentType.Content["Children"] = viewModel.Children;
            this._speedWagon.ContentTypeService.Save(contentType, User.Identity.Name);

            return RedirectToAction("Edit", new { url = contentType.Name, operation = "edited" });
        }


        [HttpPost]
        public IActionResult AddProperty(EditContentTypeViewModel viewModel)
        {
            SpeedWagonContent contentType = this._speedWagon.ContentTypeService.Get(viewModel.Url);

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

            return RedirectToAction("Edit", new { url = viewModel.Url });
        }

        public IActionResult EditProperty(string name, string property)
        {
            SpeedWagonContent contentType = this._speedWagon.ContentTypeService.Get(name);
            ContentTypeEditor[] properties = this._speedWagon.ContentTypeService.GetEditors(contentType);
            IEnumerable<SpeedWagonContent> editors = this._speedWagon.EditorService.List();

            ContentTypeEditor prop = properties.FirstOrDefault(x => x.Name == property);

            EditProperty model = new EditProperty();
            model.ContentTypeName = contentType.Name;
            model.Property = prop;
            model.AvailableEditors = SelectListHelper.GetSelectList(editors);

            return View("~/Views/SpeedWagon/ContentType/EditProperty.cshtml", model);
        }

        [HttpPost]
        public IActionResult EditProperty(EditProperty model)
        {
            if(!ModelState.IsValid)
            {

            }

            SpeedWagonContent contentType = this._speedWagon.ContentTypeService.Get(model.ContentTypeName);
            ContentTypeEditor[] properties = this._speedWagon.ContentTypeService.GetEditors(contentType);

            int index = Array.FindIndex(properties, x => x.Name == model.Property.Name);
            if(index > -1)
            {
                properties[index] = model.Property;
            }

            contentType.Content["Editors"] = properties;
            this._speedWagon.ContentTypeService.Save(contentType, User.Identity.Name);
            return RedirectToAction("Edit", new { url = model.ContentTypeName });
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

            return RedirectToAction("Edit", new { url = model.Name });
        }

        [HttpPost]
        public IActionResult MoveEditorDown(ContentTypeEditor model)
        {
            SpeedWagonContent contentType = this._speedWagon.ContentTypeService.Get(model.Name);
            this._speedWagon.ContentTypeService.MoveEditorDown(contentType, model.Editor);
            this._speedWagon.ContentTypeService.Save(contentType, User.Identity.Name);

            return RedirectToAction("Edit", new { url = model.Name });
        }

        [HttpPost]
        public IActionResult DeleteEditor(ContentTypeEditor model)
        {
            SpeedWagonContent contentType = this._speedWagon.ContentTypeService.Get(model.Name);
            this._speedWagon.ContentTypeService.DeleteEditor(contentType, model.Editor);
            this._speedWagon.ContentTypeService.Save(contentType, User.Identity.Name);

            return RedirectToAction("Edit", new { url = model.Name });
        }
    }
}

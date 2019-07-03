using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpeedWagon.Web.Interfaces;
using SpeedWagon.Web.Models.View.Editor;

namespace SpeedWagon.Web.Controllers
{
    [Authorize]
    public class SpeedWagonEditorController : Controller
    {
        private readonly ISpeedWagonAdminContext _speedWagon;

        public SpeedWagonEditorController(ISpeedWagonAdminContext speedWagon)
        {
            this._speedWagon = speedWagon;
        }

        public IActionResult List()
        {           
            EditorViewModel viewModel = new EditorViewModel();
            viewModel.Editors = this._speedWagon.EditorService.List();
            return View("~/Views/SpeedWagon/Editor/List.cshtml", viewModel);
        }

        [HttpPost]
        public IActionResult Add(EditorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Editors = this._speedWagon.EditorService.List();
                return View("~/Views/SpeedWagon/Editor/List.cshtml", model);
            }

            this._speedWagon.EditorService.Add(model.Name, User.Identity.Name);
            return RedirectToAction("List", new { Added = model.Name });
        }

        [HttpPost]
        public IActionResult Delete(DeleteEditorModel model)
        {
            this._speedWagon.EditorService.Delete(model.Name);
            return RedirectToAction("List", new { deleted = model.Name });
        }
    }
}

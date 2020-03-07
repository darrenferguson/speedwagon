using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpeedWagon.Web.Extension;
using SpeedWagon.Web.Interfaces;
using SpeedWagon.Web.Models.View.Editor;
using System.Threading.Tasks;

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

        public async Task<IActionResult> List()
        {           
            EditorViewModel viewModel = new EditorViewModel();
            viewModel.Editors = await this._speedWagon.EditorService.List();
            return View("~/Views/SpeedWagon/Editor/List.cshtml", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(EditorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Editors = await this._speedWagon.EditorService.List();
                return View("~/Views/SpeedWagon/Editor/List.cshtml", model);
            }

            this._speedWagon.EditorService.Add(model.Name, User.Identity.Name.MaskEmail());
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

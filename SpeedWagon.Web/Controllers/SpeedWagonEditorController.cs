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

        [HttpPost]
        public IActionResult Delete(DeleteEditorModel model)
        {
            this._speedWagon.EditorService.Delete(model.Name);
            return RedirectToAction("Editor", "SpeedWagon", new { deleted = model.Name });
        }
    }
}

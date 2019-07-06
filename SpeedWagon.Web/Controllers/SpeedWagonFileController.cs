using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpeedWagon.Runtime.Interfaces;
using SpeedWagon.Web.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedWagon.Web.Controllers
{
    public class SpeedWagonFileController : Controller
    {
        private readonly ISpeedWagonAdminContext _speedwagon;

        public SpeedWagonFileController(ISpeedWagonAdminContext speedwagon)
        {
            this._speedwagon = speedwagon;
        }

        public async Task<IActionResult> Upload()
        {
            string path = Request.Form["ContentPath"];
            IList<string> result = new List<string>();

            foreach (IFormFile formFile in Request.Form.Files)
            {
                if (formFile.Length > 0)
                {
                    string swPath = await this._speedwagon.FileUploadService.UploadFile(path, formFile, User.Identity.Name);
                    result.Add(swPath);
                }              
            }
            
            return Ok(new { files = result });
        }
    }
}

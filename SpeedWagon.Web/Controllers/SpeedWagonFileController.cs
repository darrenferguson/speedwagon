using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedWagon.Web.Controllers
{
    public class SpeedWagonFileController : Controller
    {

        public async Task<IActionResult> Upload()
        {

            long size = Request.Form.Files.Sum(f => f.Length);

            // full path to file in temp location
            var filePath = Path.GetTempFileName();

            IList<string> result = new List<string>();

            foreach (var formFile in Request.Form.Files)
            {
                if (formFile.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
                result.Add(formFile.FileName);
            }

            // process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(new { files = result });
        }
    }
}

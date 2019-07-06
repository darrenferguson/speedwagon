using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace SpeedWagon.Web.Interfaces
{
    public interface IFileUploadService
    {

        Task<string> UploadFile(string contentPath, IFormFile file, string user);

    }
}

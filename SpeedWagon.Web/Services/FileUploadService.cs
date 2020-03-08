using Microsoft.AspNetCore.Http;
using SpeedWagon.Interfaces;
using SpeedWagon.Runtime.Interfaces;
using SpeedWagon.Services;
using SpeedWagon.Web.Interfaces;
using System.Threading.Tasks;

namespace SpeedWagon.Web.Services
{
    public class FileUploadService : BaseSpeedWagonService, IFileUploadService
    {
        public override string Root => "uploads";

        private readonly IContentService _cachelessContentService;
        private readonly string _contentRoot;
        private readonly IFileProvider _fileProvider;
        public FileUploadService(IContentService cachelessContentService, string contentRoot, IFileProvider fileProvider) : base(contentRoot)
        {
            this._cachelessContentService = cachelessContentService;
            this._contentRoot = contentRoot;
            this._fileProvider = fileProvider;
        }

        public async Task<string> UploadFile(string parent, IFormFile file, string user)
        {
            if(parent.StartsWith("/content"))
            {
                parent = parent.Substring("/content".Length);
            }

            string urlName = RationalisePath(parent);
            CacheLessRuntimeContentService svc = ((CacheLessRuntimeContentService)this._cachelessContentService);

            string name = svc.PathMapper.RemoveInvalidFileNameChars(file.FileName);
            string path = svc.PathMapper.PathForUrl(urlName, true);
            path = path.Replace(@"\content.json", @"\" + name);

            using (var stream = await this._fileProvider.GetStream(path))
            {
                await file.CopyToAsync(stream);
            }

            return parent + "/" + name;
        }
    }
}

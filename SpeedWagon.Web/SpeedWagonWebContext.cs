using Microsoft.AspNetCore.Http;
using SpeedWagon.Interfaces;
using SpeedWagon.Models;
using SpeedWagon.Services;
using SpeedWagon.Web.Interfaces;
using SpeedWagon.Web.Models;
using System.Threading.Tasks;

namespace SpeedWagon.Web
{
    public class SpeedWagonWebContext : BaseSpeedWagonContext, ISpeedWagonWebContext
    {
        private readonly string _path;

        private readonly IContentService _cachedContentService;
       
        public SpeedWagonWebContext(string path, IContentService cachedContentService)
        {
            this._path = path;
            this._cachedContentService = cachedContentService;
        }

        public IContentService ContentService => this._cachedContentService;

        public async Task<SpeedWagonContent> ContentFor(HttpRequest request)
        {
            string url = SPEEDWAGON_HOST + "/content/" + request.Host + request.Path;
            SpeedWagonContent content =  await this._cachedContentService.GetContent(url);

            if(content == null)
            {
                return new SpeedWagonContent("Not found", request.Path);
            }

            return content;
        }

        public async Task<SpeedWagonPage> PageFor(HttpRequest request)
        {
            SpeedWagonPage model = new SpeedWagonPage();
            model.Content = await ContentFor(request);
            model.ContentService = this._cachedContentService;

            return model;

        }
    }
}
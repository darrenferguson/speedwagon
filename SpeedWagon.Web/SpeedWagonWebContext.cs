using Microsoft.AspNetCore.Http;
using SpeedWagon.Interfaces;
using SpeedWagon.Models;
using SpeedWagon.Services;
using SpeedWagon.Web.Interfaces;
using SpeedWagon.Web.Models;

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

       
        public SpeedWagonContent ContentFor(HttpRequest request)
        {
            string url = SPEEDWAGON_HOST + "/content/" + request.Host + request.Path;
            SpeedWagonContent content =  this._cachedContentService.GetContent(url);

            if(content == null)
            {
                return new SpeedWagonContent("Not found", request.Path);
            }

            return content;
        }

        public SpeedWagonPage PageFor(HttpRequest request)
        {
            SpeedWagonPage model = new SpeedWagonPage();
            model.Content = ContentFor(request);
            model.ContentService = this._cachedContentService;

            return model;

        }
    }
}
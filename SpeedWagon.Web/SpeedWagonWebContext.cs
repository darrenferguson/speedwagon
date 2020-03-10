using Microsoft.AspNetCore.Http;
using SpeedWagon.Interfaces;
using SpeedWagon.Models;
using SpeedWagon.Runtime.Interfaces;
using SpeedWagon.Web.Interfaces;
using SpeedWagon.Web.Models;
using System.Threading.Tasks;

namespace SpeedWagon.Web
{
    public class SpeedWagonWebContext : BaseSpeedWagonContext, ISpeedWagonWebContext
    {
        private readonly string _path;

        private readonly IContentService _cachedContentService;
        private readonly ISearchService _searchService;

        public SpeedWagonWebContext(string path, IContentService cachedContentService, ISearchService searchService)
        {
            this._path = path;
            this._cachedContentService = cachedContentService;
            this._searchService = searchService;
        }

        public IContentService ContentService => this._cachedContentService;

        public ISearchService SearchService => this._searchService;


        public async Task<SpeedWagonContent> ContentFor(HttpRequest request)
        {
            
            return await this.ContentFor(request, request.Path);
        }

       
        public async Task<SpeedWagonContent> ContentFor(HttpRequest request, string path)
        {
            string url = SPEEDWAGON_HOST + "/content/" + request.Host + path;
            SpeedWagonContent content = await this._cachedContentService.GetContent(url);

            if (content == null)
            {
                return new SpeedWagonContent("404NotFound", request.Path);
            }

            return content;
        }

        public async Task<SpeedWagonPage> PageFor(HttpRequest request)
        {
            SpeedWagonPage model = new SpeedWagonPage();
            model.Content = await ContentFor(request);
            model.Status = model.Content.Name == "404NotFound" ? 404 : 200;
            model.Context = this;
            model.ContentService = this.ContentService;

            return model;

        }
    }
}
using Microsoft.AspNetCore.Http;
using SpeedWagon.Interfaces;
using SpeedWagon.Models;
using SpeedWagon.Services;
using SpeedWagon.Web.Interfaces;

namespace SpeedWagon.Web
{
    public class SpeedWagonWebContext : ISpeedWagonWebContext
    {
        private readonly string _path;

        private readonly IContentService _cachedContentService;

        private readonly IContentService _cachelessContentService;

        const string SPEEDWAGON_HOST = "https://speedwagon.me";

        public SpeedWagonWebContext(string path)
        {
            this._path = path;
            this._cachelessContentService = new CacheLessRuntimeContentService(path, null);
            this._cachedContentService = new CachedRuntimeContentService(path, null);

        }

        public string Install(string user)
        {
            
            this._cachelessContentService.AddContent(new SpeedWagonContent("Content", SPEEDWAGON_HOST + "/content", "container", user));
            this._cachelessContentService.AddContent(new SpeedWagonContent("Content Types", SPEEDWAGON_HOST + "/content-types", "container", user));
            this._cachelessContentService.AddContent(new SpeedWagonContent("Editors", SPEEDWAGON_HOST + "/editors", "container", user));
            this._cachelessContentService.AddContent(new SpeedWagonContent("Text", SPEEDWAGON_HOST + "/editors/text", "editor", user));

            return this._path;
        }

        public SpeedWagonContent ContentFor(HttpRequest request)
        {
            string url = SPEEDWAGON_HOST + "/content" + request.Path;

            return this._cachelessContentService.GetContent(url);
        }
    }
}
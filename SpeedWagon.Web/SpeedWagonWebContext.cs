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

        public string Install()
        {
            this._cachelessContentService.AddContent(new SpeedWagonContent("Content", SPEEDWAGON_HOST + "/content"));
            this._cachelessContentService.AddContent(new SpeedWagonContent("ContentTypes", SPEEDWAGON_HOST + "/content-types"));
            this._cachelessContentService.AddContent(new SpeedWagonContent("DataTypes", SPEEDWAGON_HOST + "/data-types"));

            return this._path;
        }

        public SpeedWagonContent ContentFor(HttpRequest request)
        {
            string url = SPEEDWAGON_HOST + "/content" + request.Path;

            return this._cachelessContentService.GetContent(url);
        }
    }
}
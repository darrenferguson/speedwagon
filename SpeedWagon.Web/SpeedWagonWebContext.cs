using SpeedWagon.Interfaces;
using SpeedWagon.Services;
using SpeedWagon.Web.Interfaces;

namespace SpeedWagon.Web
{
    public class SpeedWagonWebContext : ISpeedWagonWebContext
    {
        private readonly string _path;

        private readonly IContentService _cachedContentService;

        private readonly IContentService _cachelessContentService;

        public SpeedWagonWebContext(string path)
        {
            this._path = path;
            this._cachelessContentService = new CacheLessRuntimeContentService(path, null);
            this._cachedContentService = new CachedRuntimeContentService(path, null);

        }

        public void Install()
        {
            
        }
    }
}

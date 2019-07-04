using Flurl;
using SpeedWagon.Runtime.Extension;

namespace SpeedWagon.Web.Services
{
    public abstract class BaseSpeedWagonService
    {
        private readonly string _contentRoot;
        public abstract string Root { get; }

        public BaseSpeedWagonService(string contentRoot)
        {
            this._contentRoot = contentRoot;
        }

        protected string RationalisePath(string path)
        {

            if (string.IsNullOrEmpty(path))
            {
                path = string.Empty;
            }

            if (path.StartsWith(Root))
            {
                path = path.Substring(Root.Length);
            }

            if (path.StartsWith("/" + Root))
            {
                path = path.Substring(Root.Length + 1);
            }

            string url = Url.Combine(this._contentRoot, Root);

            foreach (string component in path.Split('/'))
            {
                url = Url.Combine(url, component.ToUrlName());
            }

            return url;

        }
    }
}

using Moriyama.Runtime.Application;
using Newtonsoft.Json;
using SpeedWagon.Interfaces;
using SpeedWagon.Models;
using SpeedWagon.Runtime.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedWagon.Services
{
    public class CacheLessRuntimeContentService : IContentService
    {
        public event ContentAddedHandler Added;
        public virtual event ContentRemovedHandler Removed;


        public readonly IContentPathMapper PathMapper;
        private readonly object _lock;

        protected IList<string> Urls;

        private DateTime _lastUrlFlush;
        private readonly string _urlPath;


        private readonly string[] _domains;
        private readonly IFileProvider _fileProvider;

        //protected readonly ISearchService SearchService;

        private const string SITEMAP_FILE = "content-urls.json";
        
        public CacheLessRuntimeContentService(string path, string[] domains, IFileProvider fileProvider)
        {
            this._domains = domains;

            //SearchService = searchService;
            this._fileProvider = fileProvider;

            PathMapper = new ContentPathMapper(path, fileProvider);
            Urls = new List<string>();
            _lock = new object();

            _lastUrlFlush = DateTime.Now;

            _urlPath = this._fileProvider.Combine(PathMapper.ContentRootFolder("/"), SITEMAP_FILE);

            if (!this._fileProvider.Exists(_urlPath)) return;

            var urls = this._fileProvider.ReadAllText(_urlPath).Result;
            Urls = JsonConvert.DeserializeObject<IList<string>>(urls);
        }

        public async Task RefreshUrls()
        {
            IList<string> urls = new List<string>();

            var files = await this._fileProvider.List(PathMapper.ContentRootFolder("/"), "*.json", true);

            foreach (var file in files)
            {
                if (file.Contains(SITEMAP_FILE)) continue;

                var content = await FromFile(file);
                urls.Add(content.Url);
            }

            Urls = urls;
            FlushUrls();
        } 

        public void AddContent(SpeedWagonContent model)
        {
            var targetPath = PathMapper.PathForUrl(model.Url, true);
            var serialisedContent = JsonConvert.SerializeObject(model, Formatting.Indented);

            lock (_lock)
            {
                this._fileProvider.WriteAllText(targetPath, serialisedContent);

                if (!Urls.Contains(model.Url))
                    Urls.Add(model.Url);

            }
            FlushUrls();

            
            Added?.Invoke(model, new EventArgs());
        }

        public void RemoveContent(string url)
        {
            var targetPath = PathMapper.PathForUrl(url, true);

            if (!this._fileProvider.Exists(targetPath))
                return;

            // var fileInfo = new FileInfo(targetPath);
            // var directoryInfo = fileInfo.Directory;

            lock (_lock)
            {
                this._fileProvider.Delete(targetPath);
                
                if (Urls.Contains(url))
                    Urls.Remove(url);

                // CleanEmptyDirectory(directoryInfo.FullName);
            }

            FlushUrls();

            Removed?.Invoke(url, new EventArgs());
        }

        public IEnumerable<string> GetUrlList()
        {
            return Urls;
        }

        protected void FlushUrls()
        {
            // TODO: Configure flush interval
            // if (_lastUrlFlush >= DateTime.Now.AddSeconds(-30)) return;
            lock (_lock)
            {
                _lastUrlFlush = DateTime.Now;
                var urls = JsonConvert.SerializeObject(Urls, Formatting.Indented);
                this._fileProvider.WriteAllText(_urlPath, urls);
            }
        }

        protected string ProcessUrlAliases(string url)
        {
            if (this._domains == null) return url;

            foreach (string domain in this._domains)
            {
                url = url.Replace("/" + domain + "/", "/localhost/");
            }

            return url;
        }

        public async virtual Task<SpeedWagonContent> GetContent(string url)
        {
            url = ProcessUrlAliases(url);

            //if(Logger.IsDebugEnabled)
            //    Logger.Debug("Got from disk " + url);

            var contentFile = PathMapper.PathForUrl(url, false);

            if (!this._fileProvider.Exists(contentFile))
            {
                Removed?.Invoke(url, new EventArgs());
                return null;
            }

            var content = await FromFile(contentFile);


            // Added?.Invoke(content, new EventArgs());
            
            return content;
        }

        private async Task<SpeedWagonContent> FromFile(string path)
        {
            if (!this._fileProvider.Exists(path))
                return null;

            var json = await this._fileProvider.ReadAllText(path);
            return JsonConvert.DeserializeObject<SpeedWagonContent>(json);
        }

        protected string HomeUrl(SpeedWagonContent model)
        {
            var a = Urls.Where(x => model.Url.StartsWith(x)).OrderBy(x => x.Length);
            return a.First();
        }

        public async virtual Task<SpeedWagonContent> Home(SpeedWagonContent model)
        {
            return await GetContent(HomeUrl(model));
        }

        protected async Task<IEnumerable<string>> TopNavigationUrls(SpeedWagonContent model)
        {
            SpeedWagonContent home = await Home(model);
            return Urls.Where(x => x.Split('/').Length == 5 && x.StartsWith(home.Url));
        }

        public async virtual Task<IEnumerable<SpeedWagonContent>> TopNavigation(SpeedWagonContent model)
        {
            var urls = await TopNavigationUrls(model);
            IEnumerable<SpeedWagonContent> content = await FromUrls(urls);
            return content.Where(x => x != null);   
        }

        protected IEnumerable<string> ChildrenUrls(SpeedWagonContent model)
        {
            return Urls.Where(x => x.StartsWith(model.Url +"/") && x != model.Url && x.Split('/').Length == model.Url.Split('/').Length + 1);
        }

        public async virtual Task<IEnumerable<SpeedWagonContent>> Children(SpeedWagonContent model)
        {
            IEnumerable<SpeedWagonContent> content = await FromUrls(ChildrenUrls(model));
            return content.Where(x => x != null);
        }

        protected IEnumerable<string> DescendantsUrls(SpeedWagonContent model)
        {
            return Urls.Where(x => x.StartsWith(model.Url) && x != model.Url);
        }

        public async virtual Task<IEnumerable<SpeedWagonContent>> Descendants(SpeedWagonContent model)
        {
            IEnumerable<SpeedWagonContent> content = await FromUrls(DescendantsUrls(model));
            return content.Where(x => x != null);
        }

        public async virtual Task<SpeedWagonContent> Parent(SpeedWagonContent model)
        {
            string parentUrl = model.Url.Substring(0, model.Url.LastIndexOf("/"));
            IEnumerable<SpeedWagonContent> content = await FromUrls(new[] { parentUrl });

            return content.FirstOrDefault();
        }

        public async virtual Task<IEnumerable<SpeedWagonContent>> BreadCrumb(SpeedWagonContent model)
        {
            IList<SpeedWagonContent> crumb = new List<SpeedWagonContent>();
            SpeedWagonContent parent = model;

            while(parent != null && parent.Level > 0) {
                crumb.Add(parent);
                parent = await Parent(parent);
            }

            return crumb.Reverse();
        }

        public async virtual Task<IEnumerable<SpeedWagonContent>> Descendants(SpeedWagonContent model, IDictionary<string, string> filters)
        {
            var descendants = await Descendants(model);
            var filteredDescendants = new List<SpeedWagonContent>();

            foreach (var descendant in descendants)
            {
                var include = true;

                foreach (var filter in filters)
                {
                    var key = filter.Key;
                    var value = filter.Value;

                    if (
                        (descendant.Content.ContainsKey(key) && descendant.Content[key] != value)
                        ||
                        (HasProperty(descendant, key) && GetPropertyValue(descendant, key) != value)
                        )
                    {
                        include = false;
                    }
                }

                if (include)
                {
                    filteredDescendants.Add(descendant);
                }
            }
            return filteredDescendants;
        }

        private bool HasProperty(object o, string propertyName)
        {
            var property = o.GetType().GetProperty(propertyName);
            return property != null;
        }

        private string GetPropertyValue(object o, string propertyName)
        {
            var property = o.GetType().GetProperty(propertyName);
            return property.GetValue(o).ToString();
        }

        public SpeedWagonContent CreateContent(string url, IDictionary<string, object> properties)
        {
            url = ProcessUrlAliases(url);

            var content = new SpeedWagonContent(url, url);

            content.CreatorName = "User Generated";
            content.WriterName = "User Generated";

            content.RelativeUrl = new Uri(url).AbsolutePath;

            content.Type = "UserGeneratedContent";
            content.Template = string.Empty;

            content.Content = properties;

            return content;
        }

        private async Task<IEnumerable<SpeedWagonContent>> FromUrls(IEnumerable<string> urls)
        {
            var content = new List<SpeedWagonContent>();
            foreach (var url in urls)
            {
                SpeedWagonContent c = await GetContent(url);
                content.Add(c);

                // var p = Path.Combine(_pathMapper.ContentFolder(path), _pathMapper.GetContentFileName());
                // content.Add(FromFile(p));
            }

            return content;
        }

       
        //private void CleanEmptyDirectory(string path)
        //{
        //    if (!Directory.EnumerateFileSystemEntries(path).Any())
        //        Directory.Delete(path);
        //}
    }
}

using Moriyama.Runtime.Application;
using Newtonsoft.Json;
using SpeedWagon.Interfaces;
using SpeedWagon.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SpeedWagon.Services
{
    public class CacheLessRuntimeContentService : IContentService
    {
        public event ContentAddedHandler Added;
        public virtual event ContentRemovedHandler Removed;


        protected readonly IContentPathMapper PathMapper;
        private readonly object _lock;

        protected List<string> Urls;

        private DateTime _lastUrlFlush;
        private readonly string _urlPath;


        private readonly string[] _domains;

        //protected readonly ISearchService SearchService;

        private const string SITEMAP_FILE = "content-urls.json";
        
        public CacheLessRuntimeContentService(string path, string[] domains)
        {
            this._domains = domains;

            //SearchService = searchService;

            PathMapper = new ContentPathMapper(path);
            Urls = new List<string>();
            _lock = new object();

            _lastUrlFlush = DateTime.Now;

            _urlPath = Path.Combine(PathMapper.ContentRootFolder("/"), SITEMAP_FILE);

            if (!File.Exists(_urlPath)) return;

            var urls = File.ReadAllText(_urlPath);
            Urls = JsonConvert.DeserializeObject<List<string>>(urls);
        }

        public void RefreshUrls()
        {
            var urls = new List<string>();
            var files = Directory.GetFiles(PathMapper.ContentRootFolder("/"), "*.json", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                if (file.Contains(SITEMAP_FILE)) continue;

                var content = FromFile(file);
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
                File.WriteAllText(targetPath, serialisedContent);

                if (!Urls.Contains(model.Url))
                    Urls.Add(model.Url);

            }
            FlushUrls();

            //SearchService.Index(model);
      
        }

        public void RemoveContent(string url)
        {
            var targetPath = PathMapper.PathForUrl(url, true);

            if (!File.Exists(targetPath))
                return;

            var fileInfo = new FileInfo(targetPath);
            // var directoryInfo = fileInfo.Directory;

            lock (_lock)
            {
                File.Delete(targetPath);
                
                if (Urls.Contains(url))
                    Urls.Remove(url);

                // CleanEmptyDirectory(directoryInfo.FullName);
            }
            FlushUrls();

            //SearchService.Delete(url);
        }

        public IEnumerable<string> GetUrlList()
        {
            return Urls;
        }

        //public SpeedWagonContent GetContent(HttpContext context)
        //{
        //    return GetContent(GetContentUrl(context));
        //}

        //public string GetContentUrl(HttpContext context)
        //{
        //    var url = context.Request.Url.Scheme + "://" + context.Request.Url.Host + context.Request.Url.AbsolutePath;
        //    return url;
        //}

        protected void FlushUrls()
        {
            // TODO: Configure flush interval
            // if (_lastUrlFlush >= DateTime.Now.AddSeconds(-30)) return;
            lock (_lock)
            {
                _lastUrlFlush = DateTime.Now;
                var urls = JsonConvert.SerializeObject(Urls, Formatting.Indented);
                File.WriteAllText(_urlPath, urls);
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

        public virtual SpeedWagonContent GetContent(string url)
        {
            url = ProcessUrlAliases(url);

            //if(Logger.IsDebugEnabled)
            //    Logger.Debug("Got from disk " + url);

            var contentFile = PathMapper.PathForUrl(url, false);

            if (!File.Exists(contentFile))
            {
                //SearchService.Delete(url);
                return null;
            }

            var content = FromFile(contentFile);

            //SearchService.Index(content);
            
            return content;
        }

        private SpeedWagonContent FromFile(string path)
        {
            if (!File.Exists(path))
                return null;

            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<SpeedWagonContent>(json);
        }

        protected string HomeUrl(SpeedWagonContent model)
        {
            var a = Urls.Where(x => model.Url.StartsWith(x)).OrderBy(x => x.Length);
            return a.First();
        }

        public virtual SpeedWagonContent Home(SpeedWagonContent model)
        {
            return GetContent(HomeUrl(model));
        }

        protected IEnumerable<string> TopNavigationUrls(SpeedWagonContent model)
        {
            return Urls.Where(x => x.Split('/').Length == 5 && x.StartsWith(Home(model).Url));
        }

        public virtual IEnumerable<SpeedWagonContent> TopNavigation(SpeedWagonContent model)
        {
            return FromUrls(TopNavigationUrls(model)).Where(x => x != null);   
        }

        protected IEnumerable<string> ChildrenUrls(SpeedWagonContent model)
        {
            return Urls.Where(x => x.StartsWith(model.Url +"/") && x != model.Url && x.Split('/').Length == model.Url.Split('/').Length + 1);
        }

        public virtual IEnumerable<SpeedWagonContent> Children(SpeedWagonContent model)
        {
            return FromUrls(ChildrenUrls(model)).Where(x => x != null);
        }

        protected IEnumerable<string> DescendantsUrls(SpeedWagonContent model)
        {
            return Urls.Where(x => x.StartsWith(model.Url) && x != model.Url);
        }

        public virtual IEnumerable<SpeedWagonContent> Descendants(SpeedWagonContent model)
        {
            return FromUrls(DescendantsUrls(model)).Where(x => x != null);
        }

        public virtual SpeedWagonContent Parent(SpeedWagonContent model)
        {
            string parentUrl = model.Url.Substring(0, model.Url.LastIndexOf("/"));
            return FromUrls(new[] { parentUrl }).FirstOrDefault();
        }

        public virtual IEnumerable<SpeedWagonContent> BreadCrumb(SpeedWagonContent model)
        {
            IList<SpeedWagonContent> crumb = new List<SpeedWagonContent>();
            SpeedWagonContent parent = model;

            while(parent != null && parent.Level > 0) {
                crumb.Add(parent);
                parent = Parent(parent);
            }

            return crumb.Reverse();
        }

        public virtual IEnumerable<SpeedWagonContent> Descendants(SpeedWagonContent model, IDictionary<string, string> filters)
        {
            var descendants = Descendants(model);
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

        private IEnumerable<SpeedWagonContent> FromUrls(IEnumerable<string> urls)
        {
            var content = new List<SpeedWagonContent>();
            foreach (var url in urls)
            {
                content.Add(GetContent(url));

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

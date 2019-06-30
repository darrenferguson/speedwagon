using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SpeedWagon.Interfaces;

namespace Moriyama.Runtime.Application
{
    public class ContentPathMapper : IContentPathMapper
    {
        private const string ContentFileName = "content.json";

        private readonly string _basePath;

        public ContentPathMapper(string basePath)
        {
            _basePath = basePath;
        }

        public string PathForUrl(string url, bool ensure = true)
        {
            if (ensure && !Directory.Exists(_basePath))
                Directory.CreateDirectory(_basePath);

            url = StripSchemeFromUrl(url);
            
            var path = _basePath;
            var components = url.Split('/').Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            
            foreach (var component in components)
            {
                var pathComponent = RemoveInvalidFileNameChars(component);
                path = Path.Combine(path, pathComponent);

                if (ensure && !Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }

            path = Path.Combine(path, ContentFileName);

            return path;
        }

        public string RelativePath(string path)
        {
            return path.Replace(_basePath, "");
        }

        public string GetContentFileName()
        {
            return ContentFileName;
        }

        public string ContentRootFile(string relativePath)
        {
            return Path.Combine(ContentRootFolder(relativePath), ContentFileName);
        }

        public string ContentRootFolder(string relativePath)
        {
            var components = RemoveEmptyPathComponents(relativePath);

            if (components.Count > 0 && (components[0].Contains(".") || components[0].ToLower().Contains("localhost")))
            {
                return Path.Combine(_basePath, components[0]);
            }
            return _basePath;
        }

        public string ContentFolder(string relativePath)
        {
            var components = RemoveEmptyPathComponents(relativePath);
            return Path.Combine(_basePath, string.Join(@"\", components));
        }

        private List<string> RemoveEmptyPathComponents(string path)
        {
            path = path.Replace("/", @"\");
            return path.Split('\\').Where(s => !string.IsNullOrWhiteSpace(s) && s != ContentFileName).ToList();
        } 

        private string StripSchemeFromUrl(string url)
        {
            if (url.StartsWith("http://"))
                url = url.Replace("http://", "/");

            if (url.StartsWith("https://"))
                url = url.Replace("https://", "/");

            var rgx = new Regex(@"\:\d+"); // get rid of any port from the URL
            
            url = rgx.Replace(url, "");
            return url;
        }
        
        private string RemoveInvalidFileNameChars(string s)
        {
            var invalid = new string(Path.GetInvalidFileNameChars());

            foreach (var c in invalid)
            {
                s = s.Replace(c.ToString(CultureInfo.InvariantCulture), "");
            }
            return s;
        }
    }
}
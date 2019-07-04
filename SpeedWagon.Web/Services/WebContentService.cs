using SpeedWagon.Interfaces;
using SpeedWagon.Models;
using SpeedWagon.Runtime.Extension;
using SpeedWagon.Web.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpeedWagon.Web.Services
{
    public class WebContentService : IWebContentService
    {
        private readonly IContentService _cachelessContentService;
        private readonly string _contentRoot;

        private const string ROOT = "content";

        public WebContentService(IContentService cachelessContentService, string contentRoot)
        {
            this._cachelessContentService = cachelessContentService;
            this._contentRoot = contentRoot;
        }

        public SpeedWagonContent GetContent(string path)
        {
            path = SanitisePath(path);
            return this._cachelessContentService.GetContent($"{this._contentRoot}/{ROOT}/{path.ToUrlName()}");
        }

        private string SanitisePath(string path)
        {
            if(string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }
            if(path.StartsWith($"/{ROOT}"))
            {
                path = path.Replace($"/{ROOT}", string.Empty);
            }
            return path;
        }

        public IEnumerable<SpeedWagonContent> List(string path)
        {
            path = SanitisePath(path);
            return this._cachelessContentService.Children(GetContent(path));
        }

        public void Add(string parent, string name, string type, string user)
        {
            parent = SanitisePath(parent);
            string urlName = $"/{ROOT}/{parent}/{name.ToUrlName()}";

            SpeedWagonContent content = new SpeedWagonContent(name.ToTitleCasedName(), urlName, "content", user);
            string viewName = type.ToTitleCasedName() + ".cshtml";
            content.Template = "~/Views/SpeedWagon/Content/" + viewName;
            content.Type = type;

            this._cachelessContentService.AddContent(content);
        }

        public void Save(SpeedWagonContent content, string user)
        {
            content.WriterName = user;
            content.UpdateDate = DateTime.Now;
            this._cachelessContentService.AddContent(content);
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using SpeedWagon.Interfaces;
using SpeedWagon.Models;
using SpeedWagon.Runtime.Extension;
using SpeedWagon.Web.Interfaces;

namespace SpeedWagon.Web.Services
{
    public class ContentTypeService : IContentTypeService
    {

        private readonly IContentService _cachelessContentService;
        private readonly string _contentRoot;

        private const string ROOT = "editors";

        public ContentTypeService(IContentService cachelessContentService, string contentRoot)
        {
            this._cachelessContentService = cachelessContentService;
            this._contentRoot = contentRoot;
        }

        public IEnumerable<SpeedWagonContent> List()
        {
            SpeedWagonContent editorRoot = this._cachelessContentService.GetContent($"{this._contentRoot}/{ROOT}");
            return this._cachelessContentService.Children(editorRoot).OrderBy(x => x.Name);
        }

        public void Add(string name, string user)
        {
           
        }

        public void Delete(string name)
        {
            string contentPath = $"{this._contentRoot}/{ROOT}/{name.ToUrlName()}";
            this._cachelessContentService.RemoveContent(contentPath);
        }
    }
}

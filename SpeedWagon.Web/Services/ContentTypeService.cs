using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using SpeedWagon.Interfaces;
using SpeedWagon.Models;
using SpeedWagon.Runtime.Extension;
using SpeedWagon.Web.Interfaces;
using SpeedWagon.Web.Models.ContentType;

namespace SpeedWagon.Web.Services
{
    public class ContentTypeService : IContentTypeService
    {

        private readonly IContentService _cachelessContentService;
        private readonly string _contentRoot;

        private const string ROOT = "content-types";

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

            SpeedWagonContent contentType = new SpeedWagonContent(name.ToTitleCasedName(), $"{this._contentRoot}/{ROOT}/{name.ToUrlName()}", "content-type", user);
            string viewName = name.ToTitleCasedName() + ".cshtml";
            contentType.Template = "~/Views/SpeedWagon/ContentType/" + viewName;

            this._cachelessContentService.AddContent(contentType);

        }

        public void Delete(string name)
        {
            string contentPath = $"{this._contentRoot}/{ROOT}/{name.ToUrlName()}";
            this._cachelessContentService.RemoveContent(contentPath);
        }

        public SpeedWagonContent Get(string name)
        {
            return this._cachelessContentService.GetContent($"{this._contentRoot}/{ROOT}/{name.ToUrlName()}");
        }

        public ContentTypeEditor[] GetEditors(SpeedWagonContent contentType)
        {
            if (contentType.Content.ContainsKey("Editors"))
            {
                return ((JArray)contentType.Content["Editors"]).ToObject<ContentTypeEditor[]>();

            }

            return new ContentTypeEditor[] { };
        }

        public void AddEditor(SpeedWagonContent contentType, ContentTypeEditor editor)
        {
            IList<ContentTypeEditor> editors;
            if (!contentType.Content.ContainsKey("Editors"))
            {
                editors = new List<ContentTypeEditor>();
                contentType.Content.Add("Editors", editors);
            }
            else
            {
                editors = ((JArray)contentType.Content["Editors"]).ToObject<List<ContentTypeEditor>>();
            }

            editors.Add(editor);
            contentType.Content["Editors"] = editors.ToArray();
        }
    }
}

using SpeedWagon.Interfaces;
using SpeedWagon.Models;
using SpeedWagon.Runtime.Extension;
using SpeedWagon.Web.Interfaces;
using System.Linq;
using System.Collections.Generic;

namespace SpeedWagon.Web.Services
{
    public class EditorService : IEditorService
    {
        private readonly IContentService _cachelessContentService;
        private readonly string _contentRoot;

        private const string EDITOR_ROOT = "editors";

        public EditorService(IContentService cachelessContentService, string contentRoot)
        {
            this._cachelessContentService = cachelessContentService;
            this._contentRoot = contentRoot;
        }

        public IEnumerable<SpeedWagonContent> List()
        {
            SpeedWagonContent editorRoot = this._cachelessContentService.GetContent($"{this._contentRoot}/{EDITOR_ROOT}");
            return this._cachelessContentService.Children(editorRoot).OrderBy(x => x.Name);            
        }

        public void Delete(string name)
        {
            string contentPath = $"{this._contentRoot}/{EDITOR_ROOT}/{name.ToUrlName()}";
            this._cachelessContentService.RemoveContent(contentPath);
        }

        public void Add(string name, string user)
        {
            SpeedWagonContent editor = new SpeedWagonContent(name.ToTitleCasedName(), $"{this._contentRoot}/{EDITOR_ROOT}/{name.ToUrlName()}", "editor", user);

            string viewName = name.ToTitleCasedName() + ".cshtml";
            editor.Template = "~/Views/SpeedWagon/Editors/" + viewName;
            
            this._cachelessContentService.AddContent(editor);
        }
    }
}

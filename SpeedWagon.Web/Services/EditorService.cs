using SpeedWagon.Interfaces;
using SpeedWagon.Models;
using SpeedWagon.Runtime.Extension;
using SpeedWagon.Web.Interfaces;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpeedWagon.Web.Services
{
    public class EditorService : BaseSpeedWagonService, IEditorService
    {
        private readonly IContentService _cachelessContentService;
        private readonly string _contentRoot;

        public override string Root => "editors";

        public EditorService(IContentService cachelessContentService, string contentRoot) : base(contentRoot)
        {
            this._cachelessContentService = cachelessContentService;
            this._contentRoot = contentRoot;
        }

        public async Task<IEnumerable<SpeedWagonContent>> List()
        {
            SpeedWagonContent editorRoot = await this._cachelessContentService.GetContent(RationalisePath(Root));
            IEnumerable<SpeedWagonContent> children = await this._cachelessContentService.Children(editorRoot);
            return children.OrderBy(x => x.Name);            
        }

        public void Delete(string name)
        {
            this._cachelessContentService.RemoveContent(RationalisePath(name));
        }

        public void Add(string name, string user)
        {
            SpeedWagonContent editor = new SpeedWagonContent(name.ToTitleCasedName(), RationalisePath(name), "editor", user);

            string viewName = name.ToTitleCasedName().Replace("-", "") + ".cshtml";
            editor.Template = "~/Views/SpeedWagon/Editors/" + viewName;
            
            this._cachelessContentService.AddContent(editor);
        }
    }
}

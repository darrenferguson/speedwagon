using SpeedWagon.Interfaces;
using SpeedWagon.Models;
using SpeedWagon.Runtime.Extension;
using SpeedWagon.Web.Interfaces;
using SpeedWagon.Web.Models;

namespace SpeedWagon.Web
{
    public class SpeedWagonAdminContext : BaseSpeedWagonContext, ISpeedWagonAdminContext
    {
        private readonly string _path;
        private readonly IContentService _cachelessContentService;
        private readonly IContentTypeService _contentTypeService;
        private readonly IEditorService _editorService;
        private readonly IWebContentService _webContentService;

        public SpeedWagonAdminContext(
            string path, 
            IContentService cachelessContentService,
            IContentTypeService contentTypeService,
            IEditorService editorService,
            IWebContentService webContentService)
        {
            this._path = path;
            this._cachelessContentService = cachelessContentService;
            this._editorService = editorService;
            this._contentTypeService = contentTypeService;
            this._webContentService = webContentService;

        }

        public IContentService ContentService => this._cachelessContentService;

        public IEditorService EditorService => this._editorService;

        public IContentTypeService ContentTypeService => this._contentTypeService;

        public IWebContentService WebContentService => this._webContentService;

        public SpeedWagonContent GetContent(string path)
        {
            path = SPEEDWAGON_HOST + path;
            return this._cachelessContentService.GetContent(path);
        }

        public SpeedWagonPage PageFor(string path)
        {

            SpeedWagonPage model = new SpeedWagonPage();
            model.Content = GetContent(path);
            model.ContentService = this.ContentService;

            return model;
        }

        public string Install(string user)
        {            
            this._cachelessContentService.AddContent(new SpeedWagonContent("Content", SPEEDWAGON_HOST + "/content", "container", user));
            this._cachelessContentService.AddContent(new SpeedWagonContent("Content Types", SPEEDWAGON_HOST + "/content-types", "container", user));
            this._cachelessContentService.AddContent(new SpeedWagonContent("Editors", SPEEDWAGON_HOST + "/editors", "editors", user));          
            this._cachelessContentService.AddContent(new SpeedWagonContent("Users", SPEEDWAGON_HOST + "/users", "users", user));
            this._cachelessContentService.AddContent(new SpeedWagonContent(user.ToTitleCasedName(), SPEEDWAGON_HOST + "/users/" + user.ToUrlName(), "user", user));

            return this._path;
        }
    }
}
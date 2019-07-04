using SpeedWagon.Interfaces;
using SpeedWagon.Models;
using SpeedWagon.Runtime.Extension;
using SpeedWagon.Services;
using SpeedWagon.Web.Interfaces;
using SpeedWagon.Web.Models;
using SpeedWagon.Web.Services;
using System;

namespace SpeedWagon.Web
{
    public class SpeedWagonAdminContext : BaseSpeedWagonContext, ISpeedWagonAdminContext
    {
        private readonly string _path;
        private readonly IContentService _cachelessContentService;
        private readonly IContentTypeService _contentTypeService;
        private readonly IEditorService _editorService;

        public SpeedWagonAdminContext(string path)
        {
            this._path = path;
            this._cachelessContentService = new CacheLessRuntimeContentService(path, null);
            this._editorService = new EditorService(this._cachelessContentService, SPEEDWAGON_HOST);
            this._contentTypeService = new ContentTypeService(this._cachelessContentService, SPEEDWAGON_HOST);
        }

        public IContentService ContentService => this._cachelessContentService;

        public IEditorService EditorService => this._editorService;

        public IContentTypeService ContentTypeService => this._contentTypeService;

        

        public void SaveContentType(SpeedWagonContent contentType, string user)
        {
            string urlName = "/content-types/" + contentType.Name.ToUrlName();
            contentType.WriterName = user;
            this._cachelessContentService.AddContent(contentType);
        }

 
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
            
            this._cachelessContentService.AddContent(new SpeedWagonContent("Text", SPEEDWAGON_HOST + "/editors/text", "editor", user));

            this._cachelessContentService.AddContent(new SpeedWagonContent("Users", SPEEDWAGON_HOST + "/users", "users", user));
            this._cachelessContentService.AddContent(new SpeedWagonContent(user.ToTitleCasedName(), SPEEDWAGON_HOST + "/users/" + user.ToUrlName(), "user", user));

            return this._path;
        }

        public void AddContent(string parent, string name, string type, string user)
        {
            string urlName = parent + "/" + name.ToUrlName();

            SpeedWagonContent content = new SpeedWagonContent(name.ToTitleCasedName(), SPEEDWAGON_HOST + urlName, "content", user);
            string viewName = type.ToTitleCasedName() + ".cshtml";
            content.Template = "~/Views/SpeedWagon/Content/" + viewName;
            content.Type = type;
           
            this._cachelessContentService.AddContent(content);
        }

        public void SaveContent(SpeedWagonContent content, string user)
        {
            content.WriterName = user;
            content.UpdateDate = DateTime.Now;


            this._cachelessContentService.AddContent(content);
        }
    }
}
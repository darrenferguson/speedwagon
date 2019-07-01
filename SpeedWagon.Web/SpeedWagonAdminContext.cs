using SpeedWagon.Interfaces;
using SpeedWagon.Models;
using SpeedWagon.Runtime.Extension;
using SpeedWagon.Services;
using SpeedWagon.Web.Interfaces;

namespace SpeedWagon.Web
{
    public class SpeedWagonAdminContext : BaseSpeedWagonContext, ISpeedWagonAdminContext
    {
        private readonly string _path;
        private readonly IContentService _cachelessContentService;

        public SpeedWagonAdminContext(string path)
        {
            this._path = path;
            this._cachelessContentService = new CacheLessRuntimeContentService(path, null);         
        }

        public IContentService ContentService => this._cachelessContentService;

        public void AddContentType(string name, string user)
        {
            string urlName = "/content-types/" + name.ToUrlName();
           
            SpeedWagonContent contentType = new SpeedWagonContent(name.ToTitleCasedName(), SPEEDWAGON_HOST + urlName, "content-type", user);
            string viewName = name.ToTitleCasedName() + ".cshtml";
            contentType.Template = "~/Views/SpeedWagon/ContentType/" + viewName;

            this._cachelessContentService.AddContent(contentType);
        }

        public void SaveContentType(SpeedWagonContent contentType, string user)
        {
            string urlName = "/content-types/" + contentType.Name.ToUrlName();
            contentType.WriterName = user;
            this._cachelessContentService.AddContent(contentType);
        }

        public void AddEditor(string name, string user)
        {
            string urlName = "/editors/" + name.ToUrlName();
            SpeedWagonContent editor = new SpeedWagonContent(name.ToTitleCasedName(), SPEEDWAGON_HOST + urlName, "editor", user);

            string viewName = name.ToTitleCasedName() + ".cshtml";

            editor.Template = "~/Views/SpeedWagon/Editors/" + viewName;
            editor.Content.Add("HelperView", "~/Views/SpeedWagon/Editors/Helper/" + viewName);

            this._cachelessContentService.AddContent(editor);
        }

        public SpeedWagonContent GetContent(string path)
        {
            path = SPEEDWAGON_HOST + path;
            return this._cachelessContentService.GetContent(path);
        }

        public string Install(string user)
        {            
            this._cachelessContentService.AddContent(new SpeedWagonContent("Content", SPEEDWAGON_HOST + "/content", "container", user));
            this._cachelessContentService.AddContent(new SpeedWagonContent("Content Types", SPEEDWAGON_HOST + "/content-types", "container", user));
            this._cachelessContentService.AddContent(new SpeedWagonContent("Editors", SPEEDWAGON_HOST + "/editors", "container", user));
            this._cachelessContentService.AddContent(new SpeedWagonContent("Text", SPEEDWAGON_HOST + "/editors/text", "editor", user));

            return this._path;
        }

        public void AddContent(string name, string type, string user)
        {
            string urlName = "/content/" + name.ToUrlName();

            SpeedWagonContent content = new SpeedWagonContent(name.ToTitleCasedName(), SPEEDWAGON_HOST + urlName, "content", user);
            string viewName = type.ToTitleCasedName() + ".cshtml";
            content.Template = "~/Views/SpeedWagon/Content/" + viewName;
            content.Type = type;
            this._cachelessContentService.AddContent(content);
        }
    }
}
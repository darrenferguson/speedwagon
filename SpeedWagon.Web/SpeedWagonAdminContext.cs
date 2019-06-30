using SpeedWagon.Interfaces;
using SpeedWagon.Models;
using SpeedWagon.Runtime.Extension;
using SpeedWagon.Services;
using SpeedWagon.Web.Interfaces;
using System.Text.RegularExpressions;

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
            this._cachelessContentService.AddContent(new SpeedWagonContent(name.ToTitleCasedName(), SPEEDWAGON_HOST + urlName, "content-type", user));
        }

        public void AddEditor(string name, string user)
        {
            string urlName = "/editors/" + name.ToUrlName();
            this._cachelessContentService.AddContent(new SpeedWagonContent(name.ToTitleCasedName(), SPEEDWAGON_HOST + urlName, "editor", user));
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



    }
}
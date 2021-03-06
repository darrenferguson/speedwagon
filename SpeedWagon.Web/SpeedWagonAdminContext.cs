﻿using SpeedWagon.Interfaces;
using SpeedWagon.Models;
using SpeedWagon.Runtime.Extension;
using SpeedWagon.Runtime.Interfaces;
using SpeedWagon.Web.Interfaces;
using SpeedWagon.Web.Models;
using System.Threading.Tasks;

namespace SpeedWagon.Web
{
    public class SpeedWagonAdminContext : BaseSpeedWagonContext, ISpeedWagonAdminContext
    {
        private readonly string _path;
        private readonly IContentService _cachelessContentService;
        private readonly IContentTypeService _contentTypeService;
        private readonly IEditorService _editorService;
        private readonly IWebContentService _webContentService;
        private readonly IFileUploadService _fileUploadService;
        private readonly ISearchService _searchService;

        public SpeedWagonAdminContext(
            string path, 
            IContentService cachelessContentService,
            IContentTypeService contentTypeService,
            IEditorService editorService,
            IWebContentService webContentService,
            IFileUploadService fileUploadService,
            ISearchService searchService)
        {
            this._path = path;
            this._cachelessContentService = cachelessContentService;
            this._editorService = editorService;
            this._contentTypeService = contentTypeService;
            this._webContentService = webContentService;
            this._fileUploadService = fileUploadService;
            this._searchService = searchService;
        }

        public IContentService ContentService => this._cachelessContentService;

        public IEditorService EditorService => this._editorService;

        public IContentTypeService ContentTypeService => this._contentTypeService;

        public IWebContentService WebContentService => this._webContentService;

        public IFileUploadService FileUploadService => this._fileUploadService;

        public ISearchService SearchService => this._searchService;

        public async Task<SpeedWagonContent> GetContent(string path)
        {
            path = SPEEDWAGON_HOST + path;
            return await this._cachelessContentService.GetContent(path);
        }

        public async Task<SpeedWagonPage> PageFor(string path)
        {
            SpeedWagonPage model = new SpeedWagonPage();
            model.Content = await GetContent(path);
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
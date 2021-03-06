﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SpeedWagon.Interfaces;
using SpeedWagon.Models;
using SpeedWagon.Runtime.Extension;
using SpeedWagon.Web.Extension;
using SpeedWagon.Web.Interfaces;
using SpeedWagon.Web.Models.ContentType;

namespace SpeedWagon.Web.Services
{
    public class ContentTypeService : BaseSpeedWagonService, IContentTypeService
    {

        private readonly IContentService _cachelessContentService;
        private readonly string _contentRoot;

        public override string Root => "content-types";

        public ContentTypeService(IContentService cachelessContentService, string contentRoot) : base(contentRoot)
        {
            this._cachelessContentService = cachelessContentService;
            this._contentRoot = contentRoot;
        }

        public async Task<IEnumerable<SpeedWagonContent>> List()
        {
            SpeedWagonContent contentTypeRoot = await this._cachelessContentService.GetContent(RationalisePath(Root));
            IEnumerable<SpeedWagonContent> content = await this._cachelessContentService.Children(contentTypeRoot);
            return content.OrderBy(x => x.Name);
        }

        public async Task<IEnumerable<SpeedWagonContent>> ListRootTypes()
        {
            IEnumerable<SpeedWagonContent> contentTypes = await List();
            return contentTypes.Where(x => x.Content.ContainsKey("Root") && x.GetValue<bool>("Root"));
        }

        public async Task<IEnumerable<SpeedWagonContent>> ListAllowedChildren(string type)
        {
            IEnumerable<SpeedWagonContent> contentTypes = await List();
            SpeedWagonContent contentType = await Get(type);
            IEnumerable<string> children = contentType.GetValue<string[]>("Children");

            return contentTypes.Where(x => children.Contains(x.Name));
        }

        public void Add(string name, string user, bool root, string[] children)
        {

            SpeedWagonContent contentType = new SpeedWagonContent(name.ToTitleCasedName(), RationalisePath(name), "content-type", user);
            string viewName = name.ToTitleCasedName() + ".cshtml";
            contentType.Template = "~/Views/SpeedWagon/ContentType/" + viewName;

            if(children == null)
            {
                children = new string[] { };
            }

            contentType.Content.Add("Root", root);
            contentType.Content.Add("Children", children);

            this._cachelessContentService.AddContent(contentType);
        }

        public void Save(SpeedWagonContent contentType, string user)
        {
            contentType.WriterName = user;
            contentType.UpdateDate = DateTime.Now;

            this._cachelessContentService.AddContent(contentType);
        }

        public void Delete(string name)
        {
            this._cachelessContentService.RemoveContent(RationalisePath(name));
        }

        public async Task<SpeedWagonContent> Get(string name)
        {
            return await this._cachelessContentService.GetContent(RationalisePath(name));
        }

        public ContentTypeEditor[] GetEditors(SpeedWagonContent contentType)
        {
            if (contentType.Content.ContainsKey("Editors"))
            {
                return ((JArray)contentType.Content["Editors"]).ToObject<ContentTypeEditor[]>();

            } else
            {
                contentType.Content.Add("Editors", new List<ContentTypeEditor>());
            }

            return new ContentTypeEditor[] { };
        }

        private void SetEditors(SpeedWagonContent contentType, ContentTypeEditor[] editors)
        {
            if (!contentType.Content.ContainsKey("Editors"))
            {
                contentType.Content.Add("Editors", editors);
            } else
            {
                contentType.Content["Editors"] = editors;
            }
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

        public void MoveEditorUp(SpeedWagonContent contentType, string editor)
        {
            ContentTypeEditor[] editors = GetEditors(contentType);

            int index = Array.FindIndex(editors, row => row.Name == editor);
            if(index > 0)
            {
                ContentTypeEditor src = editors[index - 1];
                ContentTypeEditor dst = editors[index];

                editors[index - 1] = dst;
                editors[index] = src;

            }

            SetEditors(contentType, editors);
        }

        public void MoveEditorDown(SpeedWagonContent contentType, string editor)
        {
            ContentTypeEditor[] editors = GetEditors(contentType);

            int index = Array.FindIndex(editors, row => row.Name == editor);
            if (index < editors.Length -1)
            {
                ContentTypeEditor src = editors[index + 1];
                ContentTypeEditor dst = editors[index];

                editors[index + 1] = dst;
                editors[index] = src;

            }

            SetEditors(contentType, editors);
        }

        public void DeleteEditor(SpeedWagonContent contentType, string editor)
        {
            IEnumerable<ContentTypeEditor> editors = GetEditors(contentType).ToList();

            editors = editors.Where(x => x.Name != editor);

            SetEditors(contentType, editors.ToArray());
        }

        
    }
}

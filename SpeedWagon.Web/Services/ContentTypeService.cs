using System;
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

        public IEnumerable<SpeedWagonContent> List()
        {
            SpeedWagonContent editorRoot = this._cachelessContentService.GetContent(RationalisePath(Root));
            return this._cachelessContentService.Children(editorRoot).OrderBy(x => x.Name);
        }

        public void Add(string name, string user)
        {

            SpeedWagonContent contentType = new SpeedWagonContent(name.ToTitleCasedName(), RationalisePath(name), "content-type", user);
            string viewName = name.ToTitleCasedName() + ".cshtml";
            contentType.Template = "~/Views/SpeedWagon/ContentType/" + viewName;

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

        public SpeedWagonContent Get(string name)
        {
            return this._cachelessContentService.GetContent(RationalisePath(name));
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

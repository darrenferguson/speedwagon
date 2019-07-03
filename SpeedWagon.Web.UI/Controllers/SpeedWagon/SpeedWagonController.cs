using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Newtonsoft.Json.Linq;
using SpeedWagon.Models;
using SpeedWagon.Runtime.Extension;
using SpeedWagon.Web.Helper;
using SpeedWagon.Web.Interfaces;
using SpeedWagon.Web.Models.ContentType;
using SpeedWagon.Web.Models.View.Content;
using SpeedWagon.Web.Models.View.Editor;
using System.Collections.Generic;
using System.Linq;

namespace SpeedWagon.Web.UI.Controllers
{
    [Authorize]
    public class SpeedWagonController : Controller
    {
        private readonly ISpeedWagonAdminContext _speedWagon;
        private readonly ICompositeViewEngine _viewEngine;

        public SpeedWagonController(ISpeedWagonAdminContext speedWagon, ICompositeViewEngine viewEngine)
        {
            this._speedWagon = speedWagon;
            this._viewEngine = viewEngine;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Install()
        {
            string path = this._speedWagon.Install(User.Identity.Name);
            return View();
        }

        public IActionResult Content(string url = null)
        {
            if(string.IsNullOrEmpty(url))
            {
                url = "/content";
            }

            SpeedWagonContent contentRoot = this._speedWagon.GetContent(url);
            IEnumerable<SpeedWagonContent> contents = this._speedWagon.ContentService.Children(contentRoot);

            SpeedWagonContent contentTypeRoot = this._speedWagon.GetContent("/content-types");
            IEnumerable<SpeedWagonContent> contentTypes = this._speedWagon.ContentService.Children(contentTypeRoot);

            IList<SelectListItem> contentTypeSelct = SelectListHelper.GetSelectList(contentTypes);

            ContentViewModel viewModel = new ContentViewModel();
            viewModel.AvailableContentTypes = contentTypeSelct;
            viewModel.Content = contentRoot;
            viewModel.Contents = contents;
            viewModel.ContentService = this._speedWagon.ContentService;

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Content(ContentViewModel viewModel)
        {
            if(!ModelState.IsValid)
            {
                SpeedWagonContent contentRoot = this._speedWagon.GetContent("/content");
                IEnumerable<SpeedWagonContent> contents = this._speedWagon.ContentService.Children(contentRoot);
                
                SpeedWagonContent contentTypeRoot = this._speedWagon.GetContent("/content-types");
                IEnumerable<SpeedWagonContent> contentTypes = this._speedWagon.ContentService.Children(contentTypeRoot);

                IList<SelectListItem> contentTypeSelct = SelectListHelper.GetSelectList(contentTypes);

                viewModel.AvailableContentTypes = contentTypeSelct;
                viewModel.Content = contentRoot;
                viewModel.Contents = contents;
                viewModel.ContentService = this._speedWagon.ContentService;

                return View(viewModel);
            }

            this._speedWagon.AddContent(viewModel.Parent, viewModel.Name, viewModel.Type, User.Identity.Name);
            return RedirectToAction("Content", new { url = viewModel.Parent});
        }

        public IActionResult EditContent(string url = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                return RedirectToAction("Content");
            }

            SpeedWagonContent content = this._speedWagon.GetContent(url);
            SpeedWagonContent contentType = this._speedWagon.GetContent("/content-types/" + content.Type.ToUrlName());

            SpeedWagonContent editorRoot = this._speedWagon.GetContent("/editors");
            IEnumerable<SpeedWagonContent> editors = this._speedWagon.ContentService.Children(editorRoot);
            IEnumerable<ContentTypeEditor> properties;

            if (!contentType.Content.ContainsKey("Editors"))
            {
                properties = Enumerable.Empty<ContentTypeEditor>();
            }
            else
            {
                properties = ((JArray)contentType.Content["Editors"]).ToObject<List<ContentTypeEditor>>();
            }

            EditContentViewModel viewModel = new EditContentViewModel();
            viewModel.Url = content.RelativeUrl;
            viewModel.Content = content;
            viewModel.Editors = editors.ToArray();
            viewModel.ContentType = contentType;
            viewModel.ContentTypeProperties = properties;
            viewModel.ViewEngine = this._viewEngine;

            viewModel.Values  = content.Content.ToDictionary(k => k.Key, k => k.Value == null ? string.Empty : k.Value.ToString());

            foreach(ContentTypeEditor contentTypeEditor in properties)
            {
                if(!viewModel.Values.ContainsKey(contentTypeEditor.Name))
                {
                    viewModel.Values.Add(contentTypeEditor.Name, string.Empty);
                }
            }

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult EditContent(EditContentViewModel model)
         {
            if(!ModelState.IsValid)
            {
                return RedirectToAction("Content", new { url = model.Url });
            }

            SpeedWagonContent content = this._speedWagon.GetContent(model.Url);
            IDictionary<string, object> properties = content.Content;

            foreach(KeyValuePair<string, string> propertyValue in model.Values)
            {
                if(properties.ContainsKey(propertyValue.Key))
                {
                    properties[propertyValue.Key] = propertyValue.Value;
                }
                else
                {
                    properties.Add(propertyValue.Key, propertyValue.Value);
                }
            }
            content.Content = properties;
            this._speedWagon.SaveContent(content, User.Identity.Name);

            SpeedWagonContent parent = this._speedWagon.ContentService.Parent(content);
            return RedirectToAction("Content", new { url = parent.RelativeUrl });
        }     

        public IActionResult ContentType()
        {          
            ContentTypeViewModel viewModel = new ContentTypeViewModel();
            viewModel.ContentTypes = this._speedWagon.ContentTypeService.List();

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult ContentType(ContentTypeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ContentTypes = this._speedWagon.ContentTypeService.List();
                return View(model);
            }

            this._speedWagon.AddContentType(model.Name, User.Identity.Name);
            return RedirectToAction("EditContentType", new { id = model.Name});
        }

        public IActionResult EditContentType(string id)
        {
            id = id.ToUrlName();
            SpeedWagonContent contentType = this._speedWagon.GetContent("/content-types/" + id);

            SpeedWagonContent editorRoot = this._speedWagon.GetContent("/editors");
            IEnumerable<SpeedWagonContent> editors = this._speedWagon.ContentService.Children(editorRoot);
            
            
            EditContentTypeViewModel viewModel = new EditContentTypeViewModel();
            viewModel.ContentType = contentType;
            viewModel.Name = contentType.Name;
            viewModel.AvailableEditors = SelectListHelper.GetSelectList(editors);
            
            if (contentType.Content.ContainsKey("Editors"))
            {
                viewModel.Editors = ((JArray)contentType.Content["Editors"]).ToObject<ContentTypeEditor[]>();
                
            }

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult EditContentType(EditContentTypeViewModel viewModel)
        {
            string id = viewModel.Name;
            id = id.ToUrlName();
            SpeedWagonContent contentType = this._speedWagon.GetContent("/content-types/" + id);

            if (!ModelState.IsValid)
            {
                viewModel.ContentType = contentType;
                return View(viewModel);
            }

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
            
            editors.Add(viewModel.ContentTypeEditor);
            contentType.Content["Editors"] = editors.ToArray();

            this._speedWagon.SaveContentType(contentType, User.Identity.Name);

            return RedirectToAction("EditContentType", new { id = viewModel.Name });
        }

    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using SpeedWagon.Models;
using SpeedWagon.Web.Extension;
using SpeedWagon.Web.Helper;
using SpeedWagon.Web.Interfaces;
using SpeedWagon.Web.Models.ContentType;
using SpeedWagon.Web.Models.View.Content;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedWagon.Web.Controllers
{
    [Authorize]
    public class SpeedWagonContentController : Controller
    {
        private readonly ISpeedWagonAdminContext _speedWagon;
        private readonly ICompositeViewEngine _viewEngine;

        public SpeedWagonContentController(ISpeedWagonAdminContext speedWagon, ICompositeViewEngine viewEngine)
        {
            this._speedWagon = speedWagon;
            this._viewEngine = viewEngine;
        }

        public async Task<IActionResult> List(string url = null)
        {
            SpeedWagonContent contentRoot = await this._speedWagon.WebContentService.GetContent(url);
            IEnumerable<SpeedWagonContent> contents = await this._speedWagon.ContentService.Children(contentRoot);
            contents = contents.OrderBy(x => x.SortOrder);

            IEnumerable<SpeedWagonContent> contentTypes = null;


            if (string.IsNullOrEmpty(url) || url == "/content")
            {
                contentTypes = await this._speedWagon.ContentTypeService.ListRootTypes();
            }
            else
            {
                contentTypes  = await this._speedWagon.ContentTypeService.ListAllowedChildren(contentRoot.Type);
            }

            IList<SelectListItem> contentTypeSelct = SelectListHelper.GetSelectList(contentTypes);

            ContentViewModel viewModel = new ContentViewModel();
            viewModel.AvailableContentTypes = contentTypeSelct;
            viewModel.Content = contentRoot;
            viewModel.Contents = contents;
            viewModel.ContentService = this._speedWagon.ContentService;

            return View("~/Views/SpeedWagon/Content/List.cshtml", viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Add(ContentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                SpeedWagonContent contentRoot = await this._speedWagon.WebContentService.GetContent(viewModel.Parent);
                IEnumerable<SpeedWagonContent> contents = await this._speedWagon.ContentService.Children(contentRoot);
                IEnumerable<SpeedWagonContent> contentTypes = await this._speedWagon.ContentTypeService.List();

                IList<SelectListItem> contentTypeSelct = SelectListHelper.GetSelectList(contentTypes);

                viewModel.AvailableContentTypes = contentTypeSelct;
                viewModel.Content = contentRoot;
                viewModel.Contents = contents;
                viewModel.ContentService = this._speedWagon.ContentService;

                return View("~/Views/SpeedWagon/Content/List.cshtml", viewModel);
            }

            this._speedWagon.WebContentService.Add(viewModel.Parent, viewModel.Name, viewModel.Type, User.Identity.Name.MaskEmail());
            return RedirectToAction("List", "SpeedWagonContent",  new { url = viewModel.Parent });
        }

        public async Task<IActionResult> Edit(string url = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                return RedirectToAction("List");
            }

            SpeedWagonContent content = await this._speedWagon.GetContent(url);
            SpeedWagonContent parent = await this._speedWagon.ContentService.Parent(content);
            SpeedWagonContent contentType = await this._speedWagon.ContentTypeService.Get(content.Type);

            IEnumerable<SpeedWagonContent> editors = await this._speedWagon.EditorService.List();
            IEnumerable<ContentTypeEditor> properties = this._speedWagon.ContentTypeService.GetEditors(contentType);

            EditContentViewModel viewModel = new EditContentViewModel();

            viewModel.Url = content.RelativeUrl;
            viewModel.Parent = parent.RelativeUrl;
            viewModel.Content = content;
            viewModel.Editors = editors;
            viewModel.ContentType = contentType;
            viewModel.ContentTypeProperties = properties;
            viewModel.ViewEngine = this._viewEngine;
            viewModel.Values = this._speedWagon.WebContentService.GetValues(content, properties);

            return View("~/Views/SpeedWagon/Content/Edit.cshtml", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditContentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("List", "SpeedWagonContent", new { url = model.Url });
            }

            SpeedWagonContent content = await this._speedWagon.GetContent(model.Url);
            this._speedWagon.WebContentService.SetValues(content, model.Values);

            this._speedWagon.WebContentService.Save(content, User.Identity.Name.MaskEmail());
            SpeedWagonContent parent = await this._speedWagon.ContentService.Parent(content);

            return RedirectToAction("List", "SpeedWagonContent", new { url = parent.RelativeUrl });        
        }

        [HttpPost]
        public IActionResult Delete(ContentOperationViewModel model)
        {
            this._speedWagon.ContentService.RemoveContent(model.Url);
            return RedirectToAction("List");
        }

        [HttpPost]
        public async Task<IActionResult> MoveUp(ContentOperationViewModel model)
        {          
            SpeedWagonContent content = await this._speedWagon.ContentService.GetContent(model.Url);
            SpeedWagonContent parent = await this._speedWagon.ContentService.Parent(content);
            IEnumerable<SpeedWagonContent> children = await this._speedWagon.ContentService.Children(parent);
            children = children.OrderBy(x => x.SortOrder);

            SpeedWagonContent[] childArray = children.ToArray();
            
            for (int index = 0; index < childArray.Length; index++)
            {
                if (childArray[index].Url == content.Url && index > 0)
                {
                    childArray[index - 1].SortOrder++;
                    childArray[index].SortOrder--;

                    this._speedWagon.WebContentService.Save(childArray[index - 1], User.Identity.Name.MaskEmail());
                    this._speedWagon.WebContentService.Save(childArray[index], User.Identity.Name.MaskEmail());
                }
            }
                    
            return RedirectToAction("List");
        }

        [HttpPost]
        public async Task<IActionResult> MoveDown(ContentOperationViewModel model)
        {
            SpeedWagonContent content = await this._speedWagon.ContentService.GetContent(model.Url);
            SpeedWagonContent parent = await this._speedWagon.ContentService.Parent(content);
            IEnumerable<SpeedWagonContent> children = await this._speedWagon.ContentService.Children(parent);
            children = children.OrderBy(x => x.SortOrder);

            SpeedWagonContent[] childArray = children.ToArray();

            for (int index = 0; index < childArray.Length; index++)
            {
                if (childArray[index].Url == content.Url && index < childArray.Length -1)
                {
                    childArray[index + 1].SortOrder--;
                    childArray[index].SortOrder++;

                    this._speedWagon.WebContentService.Save(childArray[index + 1], User.Identity.Name.MaskEmail());
                    this._speedWagon.WebContentService.Save(childArray[index], User.Identity.Name.MaskEmail());
                }
            }

            return RedirectToAction("List");
        }

    }
}

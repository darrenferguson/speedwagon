using Microsoft.AspNetCore.Mvc.Rendering;
using SpeedWagon.Models;
using SpeedWagon.Web.Models.ContentType;
using System.Collections.Generic;

namespace SpeedWagon.Web.Models.View.Editor
{
    public class EditContentTypeViewModel : ContentTypeViewModel
    {
        
        public string Operation { get; set; }

        public string Url { get; set; }
        
        public SpeedWagonContent ContentType { get; set; }

        public ContentTypeEditor[] Editors { get; set; }
        
        public IList<SelectListItem> AvailableEditors { get; set; }

        public ContentTypeEditor ContentTypeEditor { get; set; }
    }
}

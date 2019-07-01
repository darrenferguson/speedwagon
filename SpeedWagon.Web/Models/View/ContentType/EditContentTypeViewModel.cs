using Microsoft.AspNetCore.Mvc.Rendering;
using SpeedWagon.Models;
using SpeedWagon.Web.Models.ContentType;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SpeedWagon.Web.Models.View.Editor
{
    public class EditContentTypeViewModel
    {
        [Required]
        public string Name { get; set; }

        public SpeedWagonContent ContentType { get; set; }

        public ContentTypeEditor[] Editors { get; set; }
        
        public IList<SelectListItem> AvailableEditors { get; set; }

        public ContentTypeEditor ContentTypeEditor { get; set; }
    }
}

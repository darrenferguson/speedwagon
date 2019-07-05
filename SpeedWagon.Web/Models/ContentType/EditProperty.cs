using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SpeedWagon.Web.Models.ContentType
{
    public class EditProperty
    {
        [HiddenInput]
        [Required]

        public string ContentTypeName { get; set; }

        public ContentTypeEditor Property { get; set; }

        public IList<SelectListItem> AvailableEditors { get; set; }
    }
}

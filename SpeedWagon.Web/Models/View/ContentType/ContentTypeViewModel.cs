using Microsoft.AspNetCore.Mvc.Rendering;
using SpeedWagon.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SpeedWagon.Web.Models.View.Editor
{
    public class ContentTypeViewModel
    {

        public IEnumerable<SpeedWagonContent> ContentTypes { get; set; }

        [Required]
        public string Name { get; set; }
     
        public bool Root { get; set; }

        public IList<SelectListItem> AvailableContentTypes { get; set; }

        public IEnumerable<string> Children { get; set; }

    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using SpeedWagon.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SpeedWagon.Web.Models.View.Content
{
    public class ContentViewModel
    {
        public IEnumerable<SpeedWagonContent> Contents { get; set; }

        public IList<SelectListItem> AvailableContentTypes { get; set; } 

        [Required]
        public string Name { get; set; }

        [Required]
        public string Type { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SpeedWagon.Web.Models.View.Content
{
    public class ContentViewModel
    {
        public IList<SelectListItem> AvailableContentTypes { get; set; } 

        [Required]
        public string Name { get; set; }

        [Required]
        public string Type { get; set; }
    }
}

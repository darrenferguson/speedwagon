using Microsoft.AspNetCore.Mvc.Rendering;
using SpeedWagon.Models;
using System.Collections.Generic;

namespace SpeedWagon.Web.Models.View.Editor
{
    public class ContentTypeViewModel
    {

        public IEnumerable<SpeedWagonContent> ContentTypes { get; set; }

        public string Name { get; set; }
     
        public bool Root { get; set; }

        public IList<SelectListItem> AvailableContentTypes { get; set; }

        public string[] Children { get; set; }

        public string CopyProperties { get; set; }

    }
}

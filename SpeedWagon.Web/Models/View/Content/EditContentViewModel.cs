using Microsoft.AspNetCore.Mvc.ViewEngines;
using SpeedWagon.Models;
using SpeedWagon.Web.Models.ContentType;
using System.Collections.Generic;

namespace SpeedWagon.Web.Models.View.Content
{
    public class EditContentViewModel
    {
        public ICompositeViewEngine ViewEngine { get; set; }

        public SpeedWagonContent Content { get; set; }

        public SpeedWagonContent ContentType { get; set; }

        public IEnumerable<ContentTypeEditor> ContentTypeProperties;

        public SpeedWagonContent[] Editors { get; set; }

        public IDictionary<string, string> Values { get; set; }

    }
}

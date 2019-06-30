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
        
    }
}

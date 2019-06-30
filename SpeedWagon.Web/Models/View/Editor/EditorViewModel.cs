using SpeedWagon.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SpeedWagon.Web.Models.View.Editor
{
    public class EditorViewModel
    {

        public IEnumerable<SpeedWagonContent> Editors { get; set; }

        [Required]
        public string Name { get; set; }
        

    }
}

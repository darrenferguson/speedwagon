using System.ComponentModel.DataAnnotations;

namespace SpeedWagon.Web.Models.View.Editor
{
    public class DeleteEditorModel
    {
        
        [Required]
        public string Name { get; set; }
        
    }
}
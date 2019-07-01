using System.ComponentModel.DataAnnotations;

namespace SpeedWagon.Web.Models.ContentType
{
    public class ContentTypeEditor
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Editor { get; set; }
    }
}

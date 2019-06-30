using System.ComponentModel.DataAnnotations;

namespace SpeedWagon.Web.Models.ContentType
{
    public class ContentTypeEditor
    {
        [Required]
        public string Name { get; set; }

        public string Editor { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace SpeedWagon.Web.Models.View.Content
{
    public class ContentOperationViewModel
    {
        [Required]
        public string Url { get; set; }
    }
}

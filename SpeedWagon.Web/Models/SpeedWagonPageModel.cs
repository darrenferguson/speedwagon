using SpeedWagon.Interfaces;
using SpeedWagon.Models;

namespace SpeedWagon.Web.Models
{
    public class SpeedWagonPage
    {
        public SpeedWagonContent Content { get; set; }

        public IContentService ContentService { get; set; }

    }
}

using SpeedWagon.Interfaces;
using SpeedWagon.Models;
using SpeedWagon.Web.Interfaces;

namespace SpeedWagon.Web.Models
{
    public class SpeedWagonPage
    {
        public SpeedWagonContent Content { get; set; }

        public IContentService ContentService { get; set; }

        public ISpeedWagonWebContext Context { get; set; }

        public int Status { get; set; }
    }
}

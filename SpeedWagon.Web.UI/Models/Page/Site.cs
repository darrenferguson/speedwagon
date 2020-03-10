using SpeedWagon.Models;
using SpeedWagon.Web.Models;
using System.Collections.Generic;

namespace SpeedWagon.Web.UI.Models.Page
{
    public class Site : SpeedWagonPage
    {
        public Site(SpeedWagonPage page)
        {
            this.Content = page.Content;
            this.ContentService = page.ContentService;
            this.Context = page.Context;
            this.Status = page.Status;

        }

        public SpeedWagonContent Home { get; set; }

        public IEnumerable<SpeedWagonContent> TopNavigation { get; set; }

    }
}

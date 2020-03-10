using SpeedWagon.Models;
using SpeedWagon.Runtime.Models;
using SpeedWagon.Web.Models;
using System.Collections.Generic;

namespace SpeedWagon.Web.UI.Models.Page
{
    public class HomePage : SpeedWagonPage
    {
        public HomePage(SpeedWagonPage page)
        {
            this.Content = page.Content;
            this.ContentService = page.ContentService;
            this.Context = page.Context;
            this.Status = page.Status;
        }

        public IEnumerable<SearchResult> Posts { get; set; }
    }
}

using SpeedWagon.Runtime.Models;
using System.Collections.Generic;

namespace SpeedWagon.New.Models.Page
{
    public class RssFeed : Site
    {
        public RssFeed(Site site) : base(site) {

            this.Home = site.Home;
            this.TopNavigation = site.TopNavigation;
        } 

        public IEnumerable<SearchResult> Posts { get; set; }
    }
}

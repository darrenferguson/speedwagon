using SpeedWagon.Runtime.Models;
using System.Collections.Generic;

namespace SpeedWagon.New.Models.Page
{
    public class HomePage : Site
    {
        public HomePage(Site site) : base(site) {

            this.Home = site.Home;
            this.TopNavigation = site.TopNavigation;
        } 

        public IEnumerable<SearchResult> Posts { get; set; }
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SpeedWagon.Runtime.Models;

namespace SpeedWagon.Web.Models.View.Search
{
    public class SearchViewModel
    {


        [Required]
        public string Term { get; set; }

        public IEnumerable<SearchResult> Results { get; set; }
    }
}

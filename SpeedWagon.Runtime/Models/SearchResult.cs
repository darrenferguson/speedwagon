using SpeedWagon.Models;

namespace SpeedWagon.Runtime.Models
{
    public class SearchResult
    {
        public double Score { get; set; }
        public string PreviewText { get; set; }
        public string Url { get; set; }
        public SpeedWagonContent Content { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace SpeedWagon.Models
{
    public class SpeedWagonContent
    {
        public SpeedWagonContent()
        {
            
        }

        public SpeedWagonContent(string name, string url)
        {
            this.Name = name;
            this.Url = url;

            if (Uri.TryCreate(url, UriKind.Absolute, out Uri u)) {
                this.RelativeUrl = u.PathAndQuery;
            } else
            {
                this.RelativeUrl = url;
            }

            this.Level = this.RelativeUrl.Split('/').Length - 2;

            this.CreateDate = DateTime.Now;
            this.UpdateDate = DateTime.Now;

            this.Content = new Dictionary<string, object>();
        }

        public SpeedWagonContent(string name, string url, string type, string author) : this(name, url)
        {
            this.Type = type;
            this.CreatorName = author;
            this.WriterName = author;
        }


        public string Name { get; set; }
        public string Type { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public string CreatorName { get; set; }
        public string WriterName { get; set; }

        public string Url { get; set; }
        public string RelativeUrl { get; set; }
        
        public IDictionary<string, object> Content { get; set; }

        public string Template { get; set; }
        
        public DateTime? CacheTime { get; set; }

        public int SortOrder { get; set; }
        public int Level { get; set; }
    }
}

using SpeedWagon.Models;
using System.Collections.Generic;

namespace SpeedWagon.Web.Interfaces
{
    public interface IWebContentService
    {
        SpeedWagonContent GetContent(string path);

        IEnumerable<SpeedWagonContent> List(string path);

        void Add(string parent, string name, string type, string user);

        void Save(SpeedWagonContent content, string user);
    }
}

using SpeedWagon.Models;
using SpeedWagon.Web.Models.ContentType;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpeedWagon.Web.Interfaces
{
    public interface IWebContentService
    {
        Task<SpeedWagonContent> GetContent(string path);

        Task<IEnumerable<SpeedWagonContent>> List(string path);

        void Add(string parent, string name, string type, string user);

        void Save(SpeedWagonContent content, string user);

        IDictionary<string, string> GetValues(SpeedWagonContent content, IEnumerable<ContentTypeEditor> properties);

        void SetValues(SpeedWagonContent content, IDictionary<string, string> values);
    }
}

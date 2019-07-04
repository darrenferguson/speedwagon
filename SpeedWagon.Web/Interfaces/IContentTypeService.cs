using SpeedWagon.Models;
using SpeedWagon.Web.Models.ContentType;
using System.Collections.Generic;

namespace SpeedWagon.Web.Interfaces
{
    public interface IContentTypeService
    {
        IEnumerable<SpeedWagonContent> List();

        SpeedWagonContent Get(string name);

        ContentTypeEditor[] GetEditors(SpeedWagonContent contentType);

        void AddEditor(SpeedWagonContent contentType, ContentTypeEditor editor);

        void Add(string name, string user);

        void Delete(string name);
    }
}

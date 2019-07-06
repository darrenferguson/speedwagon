using SpeedWagon.Models;
using SpeedWagon.Web.Models.ContentType;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpeedWagon.Web.Interfaces
{
    public interface IContentTypeService
    {
        Task<IEnumerable<SpeedWagonContent>> List();

        Task<IEnumerable<SpeedWagonContent>> ListRootTypes();

        Task<IEnumerable<SpeedWagonContent>> ListAllowedChildren(string type);

        Task<SpeedWagonContent> Get(string name);

        ContentTypeEditor[] GetEditors(SpeedWagonContent contentType);

        void AddEditor(SpeedWagonContent contentType, ContentTypeEditor editor);

        void Add(string name, string user, bool root, string[] children);

        void Delete(string name);

        void Save(SpeedWagonContent contentType, string user);

        void MoveEditorUp(SpeedWagonContent contentType, string editor);

        void MoveEditorDown(SpeedWagonContent contentType, string editor);

        void DeleteEditor(SpeedWagonContent contentType, string editor);
    }
}

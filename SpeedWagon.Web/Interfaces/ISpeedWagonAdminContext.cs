using SpeedWagon.Interfaces;
using SpeedWagon.Models;
using SpeedWagon.Web.Models;

namespace SpeedWagon.Web.Interfaces
{
    public interface ISpeedWagonAdminContext
    {

        IContentService ContentService { get; }

        IContentTypeService ContentTypeService { get; }

        IEditorService EditorService { get; }


        string Install(string user);


        SpeedWagonContent GetContent(string path);

        SpeedWagonPage PageFor(string path);


        void AddContent(string parent, string name, string type, string user);

        void SaveContent(SpeedWagonContent content, string user);
           

        void SaveContentType(SpeedWagonContent contentType, string user);
    }
}

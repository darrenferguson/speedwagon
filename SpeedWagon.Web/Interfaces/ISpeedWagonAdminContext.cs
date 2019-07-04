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

        IWebContentService WebContentService { get;  }

        string Install(string user);

        SpeedWagonContent GetContent(string path);

        SpeedWagonPage PageFor(string path);

      
    }
}

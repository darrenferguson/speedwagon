using SpeedWagon.Interfaces;
using SpeedWagon.Models;
using SpeedWagon.Web.Models;
using System.Threading.Tasks;

namespace SpeedWagon.Web.Interfaces
{
    public interface ISpeedWagonAdminContext
    {

        IContentService ContentService { get; }

        IContentTypeService ContentTypeService { get; }

        IEditorService EditorService { get; }

        IWebContentService WebContentService { get;  }

        string Install(string user);

        Task<SpeedWagonContent> GetContent(string path);

        Task<SpeedWagonPage> PageFor(string path);
   
    }
}

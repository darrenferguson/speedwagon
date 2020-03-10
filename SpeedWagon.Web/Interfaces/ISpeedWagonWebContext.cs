using Microsoft.AspNetCore.Http;
using SpeedWagon.Interfaces;
using SpeedWagon.Models;
using SpeedWagon.Runtime.Interfaces;
using SpeedWagon.Web.Models;
using System.Threading.Tasks;

namespace SpeedWagon.Web.Interfaces
{
    public interface ISpeedWagonWebContext
    {
        IContentService ContentService { get; }

        ISearchService SearchService { get; }

        Task<SpeedWagonContent> ContentFor(HttpRequest request);

        Task<SpeedWagonPage> PageFor(HttpRequest request);

        Task<SpeedWagonContent> ContentFor(HttpRequest request, string path);       
    }
}

using Microsoft.AspNetCore.Http;
using SpeedWagon.Interfaces;
using SpeedWagon.Models;
using SpeedWagon.Web.Models;
using System.Threading.Tasks;

namespace SpeedWagon.Web.Interfaces
{
    public interface ISpeedWagonWebContext
    {
        IContentService ContentService { get; }

        Task<SpeedWagonContent> ContentFor(HttpRequest request);

        Task<SpeedWagonPage> PageFor(HttpRequest request);
    }
}

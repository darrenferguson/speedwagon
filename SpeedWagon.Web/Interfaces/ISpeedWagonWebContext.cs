using Microsoft.AspNetCore.Http;
using SpeedWagon.Models;
using SpeedWagon.Web.Models;

namespace SpeedWagon.Web.Interfaces
{
    public interface ISpeedWagonWebContext
    {
        SpeedWagonContent ContentFor(HttpRequest request);

        SpeedWagonPage PageFor(HttpRequest request);
    }
}

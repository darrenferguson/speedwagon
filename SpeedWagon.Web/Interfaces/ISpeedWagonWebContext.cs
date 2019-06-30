using Microsoft.AspNetCore.Http;
using SpeedWagon.Models;

namespace SpeedWagon.Web.Interfaces
{
    public interface ISpeedWagonWebContext
    {
        SpeedWagonContent ContentFor(HttpRequest request);
    }
}

using Microsoft.AspNetCore.Http;
using SpeedWagon.Models;

namespace SpeedWagon.Web.Interfaces
{
    public interface ISpeedWagonWebContext
    {
        string Install();


        SpeedWagonContent ContentFor(HttpRequest request);
    }
}

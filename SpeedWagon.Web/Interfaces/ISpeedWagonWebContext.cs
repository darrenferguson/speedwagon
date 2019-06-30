using Microsoft.AspNetCore.Http;
using SpeedWagon.Models;

namespace SpeedWagon.Web.Interfaces
{
    public interface ISpeedWagonWebContext
    {
        string Install(string user);


        SpeedWagonContent ContentFor(HttpRequest request);
    }
}

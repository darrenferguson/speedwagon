using SpeedWagon.Models;
using System.Collections.Generic;

namespace SpeedWagon.Web.Interfaces
{
    public interface IEditorService
    {

        IEnumerable<SpeedWagonContent> List();

        void Add(string name, string user);

        void Delete(string name);


    }
}

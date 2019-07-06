using SpeedWagon.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpeedWagon.Web.Interfaces
{
    public interface IEditorService
    {

        Task<IEnumerable<SpeedWagonContent>> List();

        void Add(string name, string user);

        void Delete(string name);

    }
}

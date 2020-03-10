using System.Collections.Generic;
using System.Threading.Tasks;
using SpeedWagon.Interfaces;
using SpeedWagon.Models;
using SpeedWagon.Runtime.Models;

namespace SpeedWagon.Runtime.Interfaces
{
    public interface ISearchService
    {
        Task IndexAll(IContentService contentService);
        void Index(SpeedWagonContent model);
        void Delete(string url);

        Task<IEnumerable<SearchResult>> Search(string query);
        Task<IEnumerable<SearchResult>> Search(IDictionary<string, string> matches);

    }
}
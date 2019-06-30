using System.Collections.Generic;
using SpeedWagon.Interfaces;
using SpeedWagon.Models;

namespace SpeedWagon.Interfaces
{
    public interface ISearchService
    {
        void IndexAll(IContentService contentService);
        void Index(SpeedWagonContent model);
        void Delete(string url);

        IEnumerable<SearchResult> Search(string query);
        IEnumerable<string> Search(IDictionary<string, string> matches);

    }
}

using System;
using System.Collections.Generic;
using SpeedWagon.Interfaces;
using SpeedWagon.Models;

namespace SpeedWagon.Services.Search
{
    public class DummySearchService : ISearchService
    {
       
        public void IndexAll(IContentService contentService)
        {
        }

        public void Index(SpeedWagonContent model)
        {
            //if(Logger.IsDebugEnabled)
            //    Logger.Debug(JsonConvert.SerializeObject(model, Formatting.Indented));
        }

        public void Delete(string url)
        {
            //if (Logger.IsDebugEnabled)
            //    Logger.Debug("Delete " + url);
        }

        public IEnumerable<SearchResult> Search(string query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> Search(IDictionary<string, string> matches)
        {
            throw new NotImplementedException();
        }
    }
}
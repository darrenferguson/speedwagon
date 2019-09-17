using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpeedWagon.Interfaces;
using SpeedWagon.Models;
using SpeedWagon.Runtime.Extension;
using SpeedWagon.Runtime.Interfaces;
using SpeedWagon.Runtime.Models;

namespace SpeedWagon.Services.Search
{
    public class DummySearchService : ISearchService
    {

        private readonly IContentService _contentService;

        public DummySearchService(IContentService contentService)
        {
            this._contentService = contentService;
            contentService.Added += ContentServiceAdded;
            contentService.Removed += ContentServiceRemoved;
        }

        private void ContentServiceRemoved(string sender, EventArgs e)
        {
            Delete(sender);
        }

        private void ContentServiceAdded(SpeedWagonContent sender, EventArgs e)
        {
            Index(sender);
        }

        public Task IndexAll(IContentService contentService)
        {
            return Task.CompletedTask;
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

        public async Task<IEnumerable<SearchResult>> Search(string query)
        {
            IList<SearchResult> results = new List<SearchResult>();
            query = query.ToLower().ToUrlName();
            foreach(string url in this._contentService.GetUrlList())
            {
                if(url.Contains(query))
                {
                    SearchResult result = new SearchResult();
                    result.Url = url;
                    result.Score = 1;
                    result.PreviewText = string.Empty;
                    
                    SpeedWagonContent content = await this._contentService.GetContent(url);
                    result.Content = content;
                    results.Add(result);
                }                
            }

            return results;
        }

        public async Task<IEnumerable<string>> Search(IDictionary<string, string> matches)
        {
            throw new NotImplementedException();
        }
    }
}
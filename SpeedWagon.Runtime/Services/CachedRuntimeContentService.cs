using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using SpeedWagon.Interfaces;
using SpeedWagon.Models;
using SpeedWagon.Runtime.Interfaces;

namespace SpeedWagon.Services
{
    public class CachedRuntimeContentService : CacheLessRuntimeContentService
    {

        //[DisallowConcurrentExecution]
        //private class CacheRefresherJob : IJob
        //{
        //    public void Execute(IJobExecutionContext context)
        //    {
        //        Logger.Info(GetType().Name + " scheduled task..");

        //        var contentService = (CachedRuntimeContentService)RuntimeContext.Instance.ContentService;
        //        contentService.SanitiseCache();
        //    }
        //}

        public override event ContentRemovedHandler Removed;


        private readonly IMemoryCache _customCache;

        // private readonly CacheItemPolicy _policy;

        public CachedRuntimeContentService(string path, string[] domains, IFileProvider fileProvider) : base(path, domains, fileProvider)
        {
            _customCache = new MemoryCache(new MemoryCacheOptions {});


            //var scheduler = StdSchedulerFactory.GetDefaultScheduler();
            //scheduler.Start();

            //var job = JobBuilder.Create<CacheRefresherJob>()
            //    .WithIdentity("moriyamaCacheRefresherJob", "cacheRefresherJob")
            //    .Build();

            //var trigger = TriggerBuilder.Create()
            //    .WithIdentity("cacheRefresherJobTrigger", "cacheRefresherJobGroup")
            //    .StartNow()
            //    .WithSimpleSchedule(x => x
            //    .WithIntervalInSeconds(60)
            //    .RepeatForever())
            //    .Build();

            //scheduler.ScheduleJob(job, trigger);

      
        }

        public async Task SanitiseCache()
        {
            var stale = new List<string>();
            var hasStale = false;

            foreach (var url in Urls)
            {
                var file = PathMapper.PathForUrl(url, false);

                if (!File.Exists(file))
                    stale.Add(url);
            }

            foreach (var url in stale)
            {
                Urls.Remove(url);

                //SearchService.Delete(url);

                hasStale = true;
            }

            if (hasStale)
                FlushUrls();

            foreach (var url in Urls)
            {
                var localUrl = RemovePortFromUrl(url);
                var file = PathMapper.PathForUrl(localUrl, false);

                if (!File.Exists(file))
                    continue;

                var lastModified = File.GetLastWriteTime(file);

                SpeedWagonContent content;

                bool inCache = this._customCache.TryGetValue(localUrl, out content);

                if (inCache)
                {
                    
                    if (content.CacheTime != null && DateTime.Compare(content.CacheTime.Value, lastModified) < 0)
                    {
                        content = await base.GetContent(localUrl);
                        PlaceInCache(localUrl, content);
                    }
                }
            }
        }

        void PlaceInCache(string url, SpeedWagonContent content)
        {
            if (content == null)
                return;

            //if (Logger.IsDebugEnabled)
            //    Logger.Debug("Caching: " + url);

            content.CacheTime = DateTime.Now;
            this._customCache.Set<SpeedWagonContent>(url, content);            
        }

        public override Task<SpeedWagonContent> GetContent(string url)
        {
            url = ProcessUrlAliases(url);
            return GetCachedContent(url);
        }

        public override Task<SpeedWagonContent> Home(SpeedWagonContent model)
        {
            return (GetCachedContent(HomeUrl(model)));
        }

        private string RemovePortFromUrl(string url)
        {
            var rgx = new Regex(@"\:\d+"); // get rid of any port from the URL

            url = rgx.Replace(url, "");
            return url;
        }

        protected async virtual Task<SpeedWagonContent> GetCachedContent(string url)
        {
            url = RemovePortFromUrl(url);

            SpeedWagonContent content = null;

            if(this._customCache.TryGetValue(url, out content))
            {
                return content;
            }

            content = await base.GetContent(url);
            PlaceInCache(url, content);


            return content;
        }

        private async Task<IEnumerable<SpeedWagonContent>> FromUrls(IEnumerable<string> urls)
        {
            var contents = new List<SpeedWagonContent>();

            foreach (var url in urls)
            {
                contents.Add(await GetCachedContent(url));
            }

            return contents;
        }

        public async override Task<IEnumerable<SpeedWagonContent>> TopNavigation(SpeedWagonContent model)
        {
            IEnumerable<SpeedWagonContent> urls = await FromUrls(await TopNavigationUrls(model));
            return urls.Where(x => x != null);
        }

        public async override Task<IEnumerable<SpeedWagonContent>> Children(SpeedWagonContent model)
        {
            IEnumerable<SpeedWagonContent> content = await FromUrls(ChildrenUrls(model));
            return content.Where(x => x != null);
        }

        public async override Task<SpeedWagonContent> Parent(SpeedWagonContent model)
        {
            string parentUrl = model.Url.Substring(0, model.Url.LastIndexOf("/"));
            IEnumerable<SpeedWagonContent> content = await FromUrls(new[] { parentUrl });
            return content.FirstOrDefault();
        }

        public async override Task<IEnumerable<SpeedWagonContent>> BreadCrumb(SpeedWagonContent model)
        {
            IList<SpeedWagonContent> crumb = new List<SpeedWagonContent>();
            SpeedWagonContent parent = model;

            while (parent != null && parent.Level > 0)
            {
                crumb.Add(parent);
                parent = await Parent(parent);
            }

            return crumb.Reverse();
        }

        public async override Task<IEnumerable<SpeedWagonContent>> Descendants(SpeedWagonContent model)
        {
            IEnumerable<SpeedWagonContent> content = await FromUrls(DescendantsUrls(model));
            return content.Where(x => x != null);
        }
    }
}

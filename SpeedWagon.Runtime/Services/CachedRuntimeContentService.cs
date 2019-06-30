using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;
using SpeedWagon.Interfaces;
using SpeedWagon.Models;

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

        public CachedRuntimeContentService(string path, string[] domains) : base(path, domains)
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

        public void SanitiseCache()
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
                        content = base.GetContent(localUrl);
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

        public override SpeedWagonContent GetContent(string url)
        {
            url = ProcessUrlAliases(url);
            return GetCachedContent(url);
        }

        public override SpeedWagonContent Home(SpeedWagonContent model)
        {
            return (GetCachedContent(HomeUrl(model)));
        }

        private string RemovePortFromUrl(string url)
        {
            var rgx = new Regex(@"\:\d+"); // get rid of any port from the URL

            url = rgx.Replace(url, "");
            return url;
        }

        protected SpeedWagonContent GetCachedContent(string url)
        {
            url = RemovePortFromUrl(url);

            SpeedWagonContent content = null;

            if(this._customCache.TryGetValue(url, out content))
            {
                return content;
            }

            content = base.GetContent(url);
            PlaceInCache(url, content);

            return content;
        }

        private IEnumerable<SpeedWagonContent> FromUrls(IEnumerable<string> urls)
        {
            var contents = new List<SpeedWagonContent>();

            foreach (var url in urls)
            {
                contents.Add(GetCachedContent(url));
            }

            return contents;
        }

        public override IEnumerable<SpeedWagonContent> TopNavigation(SpeedWagonContent model)
        {
            return FromUrls(TopNavigationUrls(model)).Where(x => x != null);
        }

        public override IEnumerable<SpeedWagonContent> Children(SpeedWagonContent model)
        {
            return FromUrls(ChildrenUrls(model)).Where(x => x != null);
        }

        public override IEnumerable<SpeedWagonContent> Descendants(SpeedWagonContent model)
        {
            return FromUrls(DescendantsUrls(model)).Where(x => x != null);
        }
    }
}

using Microsoft.Extensions.Hosting;
using SpeedWagon.Services;
using SpeedWagon.Web.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SpeedWagon.Web.Services
{
    public class CacheRefreshingHostedService : IHostedService, IDisposable
    {

        private readonly ISpeedWagonWebContext _speedWagonWebContext;
        
        public CacheRefreshingHostedService(ISpeedWagonWebContext speedWagonWebContext)
        {
            this._speedWagonWebContext = speedWagonWebContext;

        }

        private Timer _timer;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (this._speedWagonWebContext.ContentService is CachedRuntimeContentService)
            {
                _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
            }

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            CachedRuntimeContentService svc = (CachedRuntimeContentService)this._speedWagonWebContext.ContentService;

            await svc.SanitiseCache();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}

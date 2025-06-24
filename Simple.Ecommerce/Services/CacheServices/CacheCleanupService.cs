using Cache.Library.Core;

namespace Simple.Ecommerce.Api.Services.CacheServices
{
    public class CacheCleanupService : BackgroundService
    {
        private readonly ICache _cache;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(2);

        public CacheCleanupService(
            ICache cache
        )
        {
            _cache = cache;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(_interval, stoppingToken);
                _cache.EvictStaleOrExpired();
                Console.WriteLine("Stale ou Expired removidos da cache!");
            }
        }
    }
}

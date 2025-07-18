using ImageFile.Library.Core.Services;
using Simple.Ecommerce.App.Interfaces.Services.ImageCleanup;

namespace Simple.Ecommerce.Api.Services.ImageServices
{
    public class CleanupImagesService : BackgroundService
    {
        private readonly IImageCleanerService _cleaner;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CleanupImagesService> _logger;

        public CleanupImagesService(
            IImageCleanerService cleaner,
            IServiceScopeFactory scopeFactory,
            ILogger<CleanupImagesService> logger
        )
        {
            _cleaner = cleaner;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Iniciando limpeza de imagens.");
                List<string> removedimages = new();
                try
                {
                    removedimages = await _cleaner.PerformCleanupAsync(DateTime.UtcNow.Subtract(TimeSpan.FromHours(6)));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro durante a limpeza de imagens.");
                }

                if (!removedimages.Any())
                {
                    _logger.LogInformation("Nenhuma imagem encontrada para limpar.");
                }
                else
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var cleanupServices = scope.ServiceProvider.GetServices<IImageCleanup>();
                        var totalUpdated = 0;

                        foreach (var cleanup in cleanupServices)
                        {
                            _logger.LogInformation($"Limpando as imagens de {cleanup.RepositoryName}");
                            var updateCount = await cleanup.RemoveImages(removedimages);
                            totalUpdated += updateCount;
                            _logger.LogInformation($"{cleanup.RepositoryName}: {updateCount} registros atualizados.");
                        }

                        _logger.LogInformation($"Limpeza concluída. Imagens removidas: {removedimages.Count}, Registros Atualizados: {totalUpdated}.");
                    }
                }
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}

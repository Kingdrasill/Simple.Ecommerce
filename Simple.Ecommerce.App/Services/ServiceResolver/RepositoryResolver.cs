using Simple.Ecommerce.App.Interfaces.Data.BaseRepository;
using Simple.Ecommerce.App.Interfaces.Services.ServiceResolver;

namespace Simple.Ecommerce.App.Services.ServiceResolver
{
    public class RepositoryResolver : IRepositoryResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public RepositoryResolver(
            IServiceProvider serviceProvider
        )
        {
            _serviceProvider = serviceProvider;
        }

        public object? GetRepository(Type entityType)
        {
            var repoType = typeof(IBaseListRepository<>).MakeGenericType(entityType);
            return _serviceProvider.GetService(repoType);
        }
    }
}

using Simple.Ecommerce.App.Interfaces.Commands.ProductCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.UnityOfWork;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.ProductEntity;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.ProductCases.Commands
{
    public class DeleteProductCommand : IDeleteProductCommand
    {
        private readonly IProductRepository _repository;
        private readonly ISaverTransectioner _saverOrTransectioner;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public DeleteProductCommand(
            IProductRepository repository,
            ISaverTransectioner unityOfWork,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _saverOrTransectioner = unityOfWork;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<bool>> Execute(int id)
        {
            var deleteResult = await _repository.Delete(id);
            if (deleteResult.IsSuccess)
            {
                var commit = await _saverOrTransectioner.SaveChanges();
                if (commit.IsFailure)
                    return commit;

                if (_useCache.Use)
                    _cacheHandler.SetItemStale<Product>();
            }

            return deleteResult;
        }
    }
}

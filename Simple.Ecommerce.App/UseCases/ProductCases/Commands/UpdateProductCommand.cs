using Simple.Ecommerce.App.Interfaces.Commands.ProductCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.Patterns.UoW;
using Simple.Ecommerce.Contracts.ProductContracts;
using Simple.Ecommerce.Domain.Entities.ProductEntity;
using Simple.Ecommerce.Domain.Objects;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.ProductCases.Commands
{
    public class UpdateProductCommand : IUpdateProductCommand
    {
        private readonly IProductRepository _repository;
        private readonly ISaverTransectioner _saverOrTransectioner;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public UpdateProductCommand(
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

        public async Task<Result<ProductResponse>> Execute(ProductRequest request)
        {
            var getProduct = await _repository.Get(request.Id);
            if (getProduct.IsFailure)
            {
                return Result<ProductResponse>.Failure(getProduct.Errors!);
            }

            var instance = new Product().Create(
                request.Id,
                request.Name,
                request.Price,
                request.Description,
                request.Stock
            );
            if (instance.IsFailure)
            {
                return Result<ProductResponse>.Failure(instance.Errors!);
            }

            var updateResult = await _repository.Create(instance.GetValue());
            if (updateResult.IsFailure)
            {
                return Result<ProductResponse>.Failure(updateResult.Errors!);
            }

            var commit = await _saverOrTransectioner.SaveChanges();
            if (commit.IsFailure)
            {
                return Result<ProductResponse>.Failure(commit.Errors!);
            }

            var product = updateResult.GetValue();

            if (_useCache.Use)
                _cacheHandler.SetItemStale<Product>();

            var response = new ProductResponse(
                product.Id,
                product.Name,
                product.Price,
                product.Description,
                product.Stock
            );

            return Result<ProductResponse>.Success(response);
        }
    }
}

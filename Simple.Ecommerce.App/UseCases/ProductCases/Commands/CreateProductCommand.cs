using Simple.Ecommerce.App.Interfaces.Commands.ProductCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.UnityOfWork;
using Simple.Ecommerce.Contracts.ProductContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.ProductEntity;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.ProductCases.Commands
{
    public class CreateProductCommand : ICreateProductCommand
    {
        private readonly IProductRepository _repository;
        private readonly ISaverTransectioner _saverOrTransectioner;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public CreateProductCommand(
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
            if (getProduct.IsSuccess)
            {
                return Result<ProductResponse>.Failure(new List<Error> { new("CreateProductCommand.AlreadyExists", "O produto já existe!") });
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

            var createResult = await _repository.Create(instance.GetValue());
            if (createResult.IsFailure)
            {
                return Result<ProductResponse>.Failure(createResult.Errors!);
            }

            var commit = await _saverOrTransectioner.SaveChanges();
            if (commit.IsFailure)
            {
                return Result<ProductResponse>.Failure(commit.Errors!);
            }

            var product = createResult.GetValue();

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

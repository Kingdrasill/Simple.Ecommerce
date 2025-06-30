using Simple.Ecommerce.App.Interfaces.Commands.ProductCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.ProductCategoryContracts;
using Simple.Ecommerce.Domain.Entities.ProductCategoryEntity;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.ProductCases.Commands
{
    public class AddCategoryProductCommand : IAddCategoryProductCommand
    {
        private readonly IProductRepository _repository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public AddCategoryProductCommand(
            IProductRepository repository,
            ICategoryRepository categoryRepository,
            IProductCategoryRepository productCategoryRepository,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
            _productCategoryRepository = productCategoryRepository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<ProductCategoryDTO>> Execute(ProductCategoryRequest request)
        {
            var getProduct = await _repository.Get(request.ProductId);
            if (getProduct.IsFailure)
            {
                return Result<ProductCategoryDTO>.Failure(getProduct.Errors!);
            }

            var getCategory = await _categoryRepository.Get(request.CategoryId);
            if (getCategory.IsFailure) 
            {
                return Result<ProductCategoryDTO>.Failure(getCategory.Errors!);
            }

            var instance = new ProductCategory().Create(
                0,
                request.ProductId,
                request.CategoryId
            );
            if (instance.IsFailure)
            {
                return Result<ProductCategoryDTO>.Failure(instance.Errors!);
            }

            var createResult = await _productCategoryRepository.Create(instance.GetValue());
            if (createResult.IsFailure) 
            {
                return Result<ProductCategoryDTO>.Failure(createResult.Errors!);
            }

            if (_useCache.Use)
                _cacheHandler.SetItemStale<ProductCategory>();

            var productCategory = createResult.GetValue();

            var response = new ProductCategoryDTO(
                productCategory.Id,
                productCategory.ProductId,
                getProduct.GetValue().Name,
                productCategory.CategoryId,
                getCategory.GetValue().Name
            );

            return Result<ProductCategoryDTO>.Success(response);
        }
    }
}

using Simple.Ecommerce.App.Interfaces.Commands.ProductCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.ProductDiscountContracts;
using Simple.Ecommerce.Domain.Entities.ProductDiscountEntity;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.UseCases.ProductCases.Commands
{
    public class AddDiscountProductCommand : IAddDiscountProductCommand
    {
        private readonly IProductRepository _repository;
        private readonly IProductDiscountRepository _productDiscountRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public AddDiscountProductCommand(
            IProductRepository repository, 
            IProductDiscountRepository productDiscountRepository, 
            IDiscountRepository discountRepository, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _productDiscountRepository = productDiscountRepository;
            _discountRepository = discountRepository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<ProductDiscountResponse>> Execute(ProductDiscountRequest request)
        {
            var getProduct = await _repository.Get(request.ProductId);
            if (getProduct.IsFailure)
            {
                return Result<ProductDiscountResponse>.Failure(getProduct.Errors!);
            }

            var getDiscount = await _discountRepository.Get(request.DiscountId);
            if (getDiscount.IsFailure)
            {
                return Result<ProductDiscountResponse>.Failure(getDiscount.Errors!);
            }

            if (getDiscount.GetValue().DiscountScope != DiscountScope.Product)
            {
                return Result<ProductDiscountResponse>.Failure(new List<Error> { new("AddDiscountProductCommand.IncorrectType.DiscountScope", "Não se pode adicionar a um produto um desconto que não seja para produto!") });
            }

            var instance = new ProductDiscount().Create(
                0,
                request.ProductId,
                request.DiscountId
            ); 
            if (instance.IsFailure)
            {
                return Result<ProductDiscountResponse>.Failure(instance.Errors!);
            }

            var createResult = await _productDiscountRepository.Create(instance.GetValue());
            if (createResult.IsFailure)
            {
                return Result<ProductDiscountResponse>.Failure(createResult.Errors!);
            }

            var productDiscount = createResult.GetValue();

            if (_useCache.Use)
                _cacheHandler.SetItemStale<ProductDiscount>();

            var response = new ProductDiscountResponse(
                productDiscount.Id,
                productDiscount.ProductId,
                productDiscount.DiscountId
            );

            return Result<ProductDiscountResponse>.Success(response);
        }
    }
}

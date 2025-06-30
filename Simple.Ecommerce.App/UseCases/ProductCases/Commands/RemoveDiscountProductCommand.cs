using Simple.Ecommerce.App.Interfaces.Commands.ProductCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Domain.Entities.ProductDiscountEntity;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.UseCases.ProductCases.Commands
{
    public class RemoveDiscountProductCommand : IRemoveDiscountProductCommand
    {
        private readonly IProductDiscountRepository _repository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public RemoveDiscountProductCommand(
            IProductDiscountRepository repository, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<bool>> Execute(int productDiscountId)
        {
            var deleteResult = await _repository.Delete(productDiscountId);

            if (deleteResult.IsSuccess && _useCache.Use)
                _cacheHandler.SetItemStale<ProductDiscount>();

            return deleteResult;
        }
    }
}

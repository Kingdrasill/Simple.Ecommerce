using Simple.Ecommerce.App.Interfaces.Commands.OrderItemCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.UseCases.OrderItemCases.Commands
{
    public class RemoveDiscountOrderItemCommand : IRemoveDiscountOrderItemCommand
    {
        private readonly IOrderItemRepository _repository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public RemoveDiscountOrderItemCommand(
            IOrderItemRepository repository, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<bool>> Execute(int orderId, int productId)
        {
            var getOrderItem = await _repository.GetByOrderIdAndProductId(orderId, productId);
            if (getOrderItem.IsFailure)
            {
                return Result<bool>.Failure(getOrderItem.Errors!);
            }

            var orderItem = getOrderItem.GetValue();
            orderItem.UpdateDiscountId(null);

            var updateResult = await _repository.Update(orderItem);
            if (updateResult.IsFailure)
            {
                return Result<bool>.Failure(updateResult.Errors!);
            }

            if (_useCache.Use)
                _cacheHandler.SetItemStale<OrderItem>();

            return Result<bool>.Success(true);
        }
    }
}

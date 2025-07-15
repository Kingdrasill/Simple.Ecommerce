using Simple.Ecommerce.App.Interfaces.Commands.OrderItemCommands;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Exceptions.ResultException;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.OrderItemCases.Commands
{
    public class RemoveAllItemsOrderItemCommand : IRemoveAllItemsOrderItemCommand
    {
        private readonly IRemoveAllItemsOrderUnitOfWork _removeAllItemsOrderUoW;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public RemoveAllItemsOrderItemCommand(
            IRemoveAllItemsOrderUnitOfWork removeAllItemsOrderUoW,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _removeAllItemsOrderUoW = removeAllItemsOrderUoW;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<bool>> Execute(int orderId)
        {
            await _removeAllItemsOrderUoW.BeginTransaction();
            try
            {
                var getOrderItems = await _removeAllItemsOrderUoW.OrderItems.GetByOrderId(orderId);
                if (getOrderItems.IsFailure)
                {
                    throw new ResultException(getOrderItems.Errors!);
                }

                foreach (var orderItem in getOrderItems.GetValue())
                {
                    var deleteResult = await _removeAllItemsOrderUoW.OrderItems.Delete(orderItem.Id, true);
                    if (deleteResult.IsFailure)
                    {
                        throw new ResultException(deleteResult.Errors!);
                    }
                }

                await _removeAllItemsOrderUoW.Commit();
                if (_useCache.Use)
                    _cacheHandler.SetItemStale<OrderItem>();

                return Result<bool>.Success(true);
            }
            catch (ResultException rex)
            {
                await _removeAllItemsOrderUoW.Rollback();
                return Result<bool>.Failure(rex.Errors);
            }
            catch (Exception ex)
            {
                await _removeAllItemsOrderUoW.Rollback();
                return Result<bool>.Failure(new List<Error> { new("RemoveAllItemsOrderItemCommand.Unknown", ex.Message) });
            }
        }
    }
}

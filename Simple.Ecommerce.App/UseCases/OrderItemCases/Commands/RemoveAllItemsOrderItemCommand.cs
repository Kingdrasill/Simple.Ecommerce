using Simple.Ecommerce.App.Interfaces.Commands.OrderItemCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.UnityOfWork;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Exceptions.ResultException;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.OrderItemCases.Commands
{
    public class RemoveAllItemsOrderItemCommand : IRemoveAllItemsOrderItemCommand
    {
        private readonly IOrderItemRepository _repository;
        private readonly ISaverTransectioner _saverOrTransectioner;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public RemoveAllItemsOrderItemCommand(
            IOrderItemRepository repository,
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

        public async Task<Result<bool>> Execute(int orderId)
        {
            await _saverOrTransectioner.BeginTransaction();
            try
            {
                var getOrderItems = await _repository.GetByOrderId(orderId);
                if (getOrderItems.IsFailure)
                {
                    throw new ResultException(getOrderItems.Errors!);
                }

                foreach (var orderItem in getOrderItems.GetValue())
                {
                    var deleteResult = await _repository.Delete(orderItem.Id);
                    if (deleteResult.IsFailure)
                    {
                        throw new ResultException(deleteResult.Errors!);
                    }
                }

                await _saverOrTransectioner.Commit();

                if (_useCache.Use)
                    _cacheHandler.SetItemStale<OrderItem>();

                return Result<bool>.Success(true);
            }
            catch (ResultException rex)
            {
                await _saverOrTransectioner.Rollback();
                return Result<bool>.Failure(rex.Errors);
            }
            catch (Exception ex)
            {
                await _saverOrTransectioner.Rollback();
                return Result<bool>.Failure(new List<Error> { new("RemoveAllItemsOrderItemCommand.Unknown", ex.Message) });
            }
        }
    }
}

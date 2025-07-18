using Simple.Ecommerce.App.Interfaces.Commands.OrderCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.OrderContracts.PaymentInformations;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Enums.OrderLock;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.OrderCases.Commands
{
    public class RemovePaymentMethodOrderCommand : IRemovePaymentMethodOrderCommand
    {
        private readonly IOrderRepository _repository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public RemovePaymentMethodOrderCommand(
            IOrderRepository repository, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<bool>> Execute(int orderId)
        {
            var getOrder = await _repository.Get(orderId);
            if (getOrder.IsFailure)
            {
                return Result<bool>.Failure(getOrder.Errors!);
            }
            var order = getOrder.GetValue();

            if (order.OrderLock is not (OrderLock.Unlock or OrderLock.LockPrice))
            {
                return Result<bool>.Failure(new List<Error> { new("OrderPaymentInformationResponse.OrderLocked", "O pedido está bloqueado à mudança de dados do pedido.") });
            }

            if (order.PaymentInformation is null)
            {
                return Result<bool>.Success(true);
            }

            var deletePaymentMethodResult = await _repository.DeletePaymentMethod(order.Id);
            if (deletePaymentMethodResult.IsSuccess)
            {
                return Result<bool>.Failure(deletePaymentMethodResult.Errors!);
            }

            if (_useCache.Use)
                _cacheHandler.SetItemStale<Order>();

            return deletePaymentMethodResult;
        }
    }
}

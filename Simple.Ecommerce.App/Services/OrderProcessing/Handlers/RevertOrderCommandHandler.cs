using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.OrderProcessing;
using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;
using Simple.Ecommerce.App.Services.OrderProcessing.ChainOfResponsibility;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Enums.OrderLock;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Exceptions.ResultException;
using Simple.Ecommerce.Domain.OrderProcessing.Commands;
using Simple.Ecommerce.Domain.OrderProcessing.Events.OrderEvent;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Handlers
{
    public class RevertOrderCommandHandler : IOrderProcessingCommandHandler<RevertOrderCommand, Result<bool>>
    {
        private readonly IConfirmedOrderUnitOfWork _canceledOrderUoW;
        private readonly IOrderProcessingDispatcher _orderDispatcher;
        private readonly IOrderProcessingChain _orderChain;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public RevertOrderCommandHandler(
            IConfirmedOrderUnitOfWork canceledOrderUoW,
            IOrderProcessingDispatcher orderDispatcher, 
            IOrderProcessingChain orderChain, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _canceledOrderUoW = canceledOrderUoW;
            _orderDispatcher = orderDispatcher;
            _orderChain = orderChain;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<bool>> Handle(RevertOrderCommand command)
        {
            Console.WriteLine($"\n[ProcessCanceledOrderCommandHandler] Começando o cancelamento do pedido {command.OrderId}.");
            await _canceledOrderUoW.BeginTransaction();
            try
            {
                var getOrder = await _canceledOrderUoW.Orders.Get(command.OrderId);
                if (getOrder.IsFailure)
                {
                    Console.WriteLine($"[ProcessCanceledOrderCommandHandler] Falha ao pegar os dados do pedido {command.OrderId}.");
                    throw new ResultException(getOrder.Errors!);
                }
                var order = getOrder.GetValue();

                if (order.OrderLock is OrderLock.Unlock)
                {
                    Console.WriteLine($"[ProcessCanceledOrderCommandHandler] O pedido {command.OrderId} não pode ser revertido no seu estado atual.");
                    order.UpdateStatus("Failed Reverted", order.OrderLock);
                    var updateResult = await _canceledOrderUoW.Orders.Update(order);
                    if (updateResult.IsFailure)
                    {
                        throw new ResultException(updateResult.Errors!);
                    }
                    return Result<bool>.Success(true);
                }
                else if (order.OrderLock is not OrderLock.LockPrice)
                {
                    Console.WriteLine($"[ProcessCanceledOrderCommandHandler] O pedido {command.OrderId} já foi pago não se pode reverter ele.");
                    throw new ResultException(new Error("ProcessCanceledOrderCommandHandler.OrderCompleted", $"O pedido {command.OrderId} já foi pago não se pode reverter ele."));
                }

                order.UpdateStatus("Revert", OrderLock.Unlock, false);

                var getUser = await _canceledOrderUoW.Users.Get(order.UserId);
                if (getUser.IsFailure)
                {
                    Console.WriteLine($"[ProcessCanceledOrderCommandHandler] Falha ao pegar os dados do usuária {order.UserId}");
                    throw new ResultException(getUser.Errors!);
                }


            }
            catch (ResultException rex)
            {
                await _canceledOrderUoW.Rollback();

                Console.WriteLine($"[ProcessCanceledOrderCommandHandler] Falha ao reverter o pedido {command.OrderId}.");
                var getOrder = await _canceledOrderUoW.Orders.Get(command.OrderId);
                if (getOrder.IsSuccess)
                {
                    var order = getOrder.GetValue();
                    order.UpdateStatus("Failed Reverted", order.OrderLock);
                    await _canceledOrderUoW.Orders.Update(order);
                    await _orderDispatcher.Dispatch(new OrderStatusChangedEvent(order.Id, order.Status));
                }

                return Result<bool>.Failure(rex.Errors);
            }
            catch (Exception ex)
            {
                await _canceledOrderUoW.Rollback();

                Console.WriteLine($"[ProcessCanceledOrderCommandHandler] Falha ao reverter o pedido {command.OrderId}.");
                var getOrder = await _canceledOrderUoW.Orders.Get(command.OrderId);
                if (getOrder.IsSuccess)
                {
                    var order = getOrder.GetValue();
                    order.UpdateStatus("Failed Reverted", order.OrderLock);
                    await _canceledOrderUoW.Orders.Update(order);
                    await _orderDispatcher.Dispatch(new OrderStatusChangedEvent(order.Id, order.Status));
                }

                return Result<bool>.Failure(new List<Error> { new("ProcessConfirmedOrderCommandHandler.Unknown", ex.Message) });
            }
            return Result<bool>.Failure(new());
        }
    }
}

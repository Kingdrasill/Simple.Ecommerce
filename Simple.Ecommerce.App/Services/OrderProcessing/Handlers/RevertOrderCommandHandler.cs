using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.OrderProcessing;
using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;
using Simple.Ecommerce.App.Services.OrderProcessing.Processor;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Enums.OrderLock;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Exceptions.ResultException;
using Simple.Ecommerce.Domain.OrderProcessing.Commands;
using Simple.Ecommerce.Domain.OrderProcessing.Events.OrderEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Models;
using Simple.Ecommerce.Domain.OrderProcessing.ReadModels;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Handlers
{
    public class RevertOrderCommandHandler : IOrderProcessingCommandHandler<RevertOrderCommand, Result<bool>>
    {
        private readonly IRevertedOrderUnitOfWork _revertedOrderUoW;
        private readonly IOrderProcessingDispatcher _orderDispatcher;
        private readonly OrderRevertProcessor _orderReverter;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public RevertOrderCommandHandler(
            IRevertedOrderUnitOfWork revertedOrderUoW,
            IOrderProcessingDispatcher orderDispatcher, 
            OrderRevertProcessor orderReverter,
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _revertedOrderUoW = revertedOrderUoW;
            _orderDispatcher = orderDispatcher;
            _orderReverter = orderReverter;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<bool>> Handle(RevertOrderCommand command)
        {
            Console.WriteLine($"\n[RevertOrderCommandHandler] Começando o cancelamento do pedido {command.OrderId}.");
            await _revertedOrderUoW.BeginTransaction();
            try
            {
                // Pegando os dados do pedido
                var getOrder = await _revertedOrderUoW.Orders.Get(command.OrderId);
                if (getOrder.IsFailure)
                {
                    Console.WriteLine($"[RevertOrderCommandHandler] Falha ao pegar os dados do pedido {command.OrderId}.");
                    throw new ResultException(getOrder.Errors!);
                }
                var order = getOrder.GetValue();

                // Verificando a trava do pedido
                if (order.OrderLock is OrderLock.Unlock)
                {
                    Console.WriteLine($"[RevertOrderCommandHandler] O pedido {command.OrderId} não pode ser revertido no seu estado atual.");
                    throw new ResultException(new Error("RevertOrderCommandHandler.Status", $"O pedido {command.OrderId} não pode ser revertido no seu estado atual."));
                }
                else if (order.OrderLock is not OrderLock.LockPrice)
                {
                    Console.WriteLine($"[RevertOrderCommandHandler] O pedido {command.OrderId} já foi pago não se pode reverter ele.");
                    throw new ResultException(new Error("RevertOrderCommandHandler.OrderCompleted", $"O pedido {command.OrderId} já foi pago não se pode reverter ele."));
                }

                // Atualizando o status do pedido
                order.UpdateStatus("Revert", OrderLock.Unlock, confirmation: false);

                // Buscando os detalhes do pedido e atualizando os que não são afetados por eventos
                var orderInProcessResult = await RecreateOrderInProcess(order);
                if (orderInProcessResult.IsFailure)
                {
                    Console.WriteLine($"[RevertOrderCommandHandler] Falha ao recriar o pedido {command.OrderId}.");
                    throw new ResultException(orderInProcessResult.Errors!);
                }
                var orderInProcess = orderInProcessResult.GetValue();

                // Buscando os eventos de processamento do pedido
                var eventsResult = await GetProcessedEvents(command.OrderId);
                if (eventsResult.IsFailure)
                {
                    Console.WriteLine($"[RevertOrderCommandHandler] Falha ao buscar os eventos de processamento do pedido {command.OrderId}.");
                    throw new ResultException(eventsResult.Errors!);
                }
                var events = eventsResult.GetValue();

                // Mandando evento de reversão iniciado
                Console.WriteLine($"[RevertOrderCommandHandler] Revertendo o pedido {command.OrderId}.");
                await _orderDispatcher.Dispatch(new OrderRevertingStartedEvent(
                    order.Id,
                    order.UserId,
                    order.OrderType,
                    order.Address,
                    order.PaymentInformation,
                    order.OrderLock,
                    order.OrderDate!.Value,
                    orderInProcess.OriginalTotalPrice,
                    orderInProcess.TotalDiscount,
                    orderInProcess.ShippingFee,
                    orderInProcess.TaxAmount,
                    order.Status,
                    orderInProcess.Items.Select(item => new OrderRevertItemEntry(
                        item.Id,
                        item.ProductId,
                        item.ProductName,
                        item.Quantity,
                        item.CurrentPrice,
                        item.DiscountAmount,
                        item.AppliedDiscount is null 
                            ? null
                            : (item.AppliedDiscount.DiscountId, item.AppliedDiscount.DiscountName)
                    )).ToList(),
                    orderInProcess.FreeItems.Select(item => new OrderRevertItemEntry(
                        item.OriginalOrderItemId,
                        item.ProductId,
                        item.ProductName,
                        item.Quantity,
                        item.CurrentPrice,
                        item.AmountDiscountedPrice,
                        item.AppliedDiscount is null
                            ? null
                            : (item.AppliedDiscount.DiscountId, item.AppliedDiscount.DiscountName)
                    )).ToList(),
                    orderInProcess.Bundles.Select(bundle => new OrderRevertBundle(
                        bundle.DiscountId,
                        bundle.DiscountName,
                        bundle.Id,
                        bundle.Items.Sum(item => item.Quantity * item.AmountDiscountedPrice),
                        bundle.Items.Select(item => new OrderRevertBundleItemEntry(
                            item.OriginalOrderItemId,
                            item.ProductId,
                            item.ProductName,
                            item.Quantity,
                            item.CurrentPrice,
                            item.AmountDiscountedPrice
                        )).ToList()
                    )).ToList(),
                    orderInProcess.AppliedDiscount is null
                        ? null
                        : (orderInProcess.AppliedDiscount.DiscountId, orderInProcess.AppliedDiscount.DiscountName)
                ));

                var revertingResult = await _orderReverter.Revert(orderInProcess, events);
                if (revertingResult.IsFailure)
                {
                    throw new ResultException(revertingResult.Errors!);
                }

                // Atualizando os items originas do pedido
                var updateResult = await UpdateOriginalItems(orderInProcess);
                if (updateResult.IsFailure)
                {
                    throw new ResultException(updateResult.Errors!);
                }

                // Removendo os items criados pelo processamento do pedido
                var removeResult = await RemoveUnusedItems(order.Id, orderInProcess);
                if (removeResult.IsFailure)
                {
                    throw new ResultException(removeResult.Errors!);
                }

                var uncommitedEvents = orderInProcess.GetUncommittedEvents();
                if (uncommitedEvents.Any())
                {
                    await _orderDispatcher.Dispatch(uncommitedEvents);
                }


                // Atualizando o status do pedido para Revertido
                order.UpdateStatus("Reverted", order.OrderLock, newTotalPrice: orderInProcess.CurrentTotalPrice);

                // Enviando o evento de pedido revertido
                Console.WriteLine($"[RevertOrderCommandHandler] O pedido {command.OrderId} foi revertido com sucesso.");
                await _orderDispatcher.Dispatch(new OrderRevertedEvent(order.Id, order.Status, order.OrderLock, order.TotalPrice!.Value));

                // Atualiazando o pedido
                var updateOrderResult = await _revertedOrderUoW.Orders.Update(order);
                if (updateOrderResult.IsFailure)
                {
                    throw new ResultException(updateOrderResult.Errors!);
                }

                // Salvando as alterações no pedido e nos itens do pedido
                await _revertedOrderUoW.Commit();
                if (_useCache.Use)
                {
                    _cacheHandler.SetItemStale<Order>();
                    _cacheHandler.SetItemStale<OrderItem>();
                }

                return Result<bool>.Success(true);
            }
            catch (ResultException rex)
            {
                await _revertedOrderUoW.Rollback();

                Console.WriteLine($"[RevertOrderCommandHandler] Falha ao reverter o pedido {command.OrderId}.");
                var getOrder = await _revertedOrderUoW.Orders.Get(command.OrderId);
                if (getOrder.IsSuccess)
                {
                    var order = getOrder.GetValue();
                    order.UpdateStatus("Failed Reverted", order.OrderLock);
                    await _revertedOrderUoW.Orders.Update(order);
                    await _orderDispatcher.Dispatch(new OrderStatusChangedEvent(order.Id, order.Status));
                }

                return Result<bool>.Failure(rex.Errors);
            }
            catch (Exception ex)
            {
                await _revertedOrderUoW.Rollback();

                Console.WriteLine($"[RevertOrderCommandHandler] Falha ao reverter o pedido {command.OrderId}.");
                var getOrder = await _revertedOrderUoW.Orders.Get(command.OrderId);
                if (getOrder.IsSuccess)
                {
                    var order = getOrder.GetValue();
                    order.UpdateStatus("Failed Reverted", order.OrderLock);
                    await _revertedOrderUoW.Orders.Update(order);
                    await _orderDispatcher.Dispatch(new OrderStatusChangedEvent(order.Id, order.Status));
                }

                return Result<bool>.Failure(new List<Error> { new("RevertOrderCommandHandler.Unknown", ex.Message) });
            }
        }

        private async Task<Result<OrderInProcess>> RecreateOrderInProcess(Order order)
        {
            var orderDetail = await _revertedOrderUoW.OrderDetails.GetOrderDetails(order.Id);
            if (orderDetail is null)
            {
                return Result<OrderInProcess>.Failure(new List<Error> { new("RevertOrderCommandHandler.OrderDetail", $"Não foi encontrado os detalhes do pedido {order.Id}.") });
            }

            if (orderDetail.Status is not ("Processed" or "Failed Reverted"))
            {
                return Result<OrderInProcess>.Failure(new List<Error> { new("RevertOrderCommandHandler.OrderDetail", $"O pedido {order.Id} não está num estado que pode ser revertido.") });
            }

            var freeItems = new List<AppliedDiscountItem>();
            var bundles = new List<AppliedBundle>();
            var items = new List<OrderItemInProcess>();

            var detailItems = orderDetail.Items;
            var discountIds = detailItems
                .Where(di => di.AppliedDiscount.HasValue)
                .Select(di => di.AppliedDiscount!.Value.DiscountId)
                .Distinct()
                .ToList();
            if (orderDetail.AppliedDiscount is not null)
                discountIds.Add(orderDetail.AppliedDiscount.Value.DiscountId);

            var getDiscounts = await _revertedOrderUoW.Discounts.GetByDiscountIds(discountIds);
            if (getDiscounts.IsFailure)
            {
                return Result<OrderInProcess>.Failure(getDiscounts.Errors!);
            }
            var discountsMap = getDiscounts.GetValue().ToDictionary(d => d.Id);

            var isFreeItems = detailItems.Where(di => di.IsFreeItem).ToList();
            foreach (var item in isFreeItems)
            {
                freeItems.Add(new AppliedDiscountItem(
                    item.OrderItemId,
                    item.ProductId,
                    item.ProductName,
                    item.Quantity,
                    item.CurrentPrice,
                    item.AmountDiscountedPrice,
                    new AppliedDiscountDetail(
                        item.AppliedDiscount!.Value.DiscountId,
                        item.AppliedDiscount!.Value.DiscountName,
                        discountsMap[item.AppliedDiscount!.Value.DiscountId].DiscountType
                    )
                ));
            }

            var isBundleItems = detailItems.Where(di => di.IsBundleItem).ToList();
            foreach (var item in isBundleItems)
            { 
                var bundle = bundles.FirstOrDefault(di => di.Id == item.BundleId);

                if (bundle is null)
                {
                    bundles.Add(new AppliedBundle(
                        item.BundleId!.Value,
                        item.AppliedDiscount!.Value.DiscountId,
                        item.AppliedDiscount!.Value.DiscountName,
                        discountsMap[item.AppliedDiscount!.Value.DiscountId].DiscountType,
                        new List<AppliedDiscountItem>
                        {
                            new AppliedDiscountItem(
                                item.OrderItemId,
                                item.ProductId,
                                item.ProductName,
                                item.Quantity,
                                item.CurrentPrice,
                                item.AmountDiscountedPrice,
                                new AppliedDiscountDetail(
                                    item.AppliedDiscount!.Value.DiscountId,
                                    item.AppliedDiscount!.Value.DiscountName,
                                    discountsMap[item.AppliedDiscount!.Value.DiscountId].DiscountType
                                ))
                        }
                    ));
                }
                else
                {
                    bundle.Items.Add(new AppliedDiscountItem(
                        item.OrderItemId,
                        item.ProductId,
                        item.ProductName,
                        item.Quantity,
                        item.CurrentPrice,
                        item.AmountDiscountedPrice,
                        new AppliedDiscountDetail(
                            item.AppliedDiscount!.Value.DiscountId,
                            item.AppliedDiscount!.Value.DiscountName,
                            discountsMap[item.AppliedDiscount!.Value.DiscountId].DiscountType
                        )));
                }
            }

            var otherItems = detailItems
                .Where(di => !di.IsFreeItem && !di.IsBundleItem)
                .ToList();
            foreach (var item in otherItems)
            {
                AppliedDiscountDetail? appliedDiscount = null;
                if (item.AppliedDiscount.HasValue)
                {
                    appliedDiscount = new AppliedDiscountDetail(
                        item.AppliedDiscount!.Value.DiscountId,
                        item.AppliedDiscount!.Value.DiscountName,
                        discountsMap[item.AppliedDiscount!.Value.DiscountId].DiscountType
                    );
                }
                items.Add(new OrderItemInProcess(
                    item.OrderItemId,
                    item.ProductId,
                    item.ProductName,
                    item.Quantity,
                    item.UnitPrice,
                    item.CurrentPrice,
                    item.AmountDiscountedPrice,
                    appliedDiscount
                ));
            }

            var orderInProcess = new OrderInProcess(
                order.Id,
                order.UserId,
                order.OrderType,
                order.Address,
                order.PaymentInformation,
                order.OrderLock,
                orderDetail.CurrentTotal,
                items,
                freeItems,
                bundles,
                orderDetail.AppliedDiscount is null 
                    ? null
                    : new AppliedDiscountDetail(
                        orderDetail.AppliedDiscount.Value.DiscountId,
                        orderDetail.AppliedDiscount.Value.DiscountName,
                        discountsMap[orderDetail.AppliedDiscount.Value.DiscountId].DiscountType
                    ),
                orderDetail.AmountDiscounted,
                orderDetail.ShippingFee,
                orderDetail.TaxAmount,
                order.Status
            );

            return Result<OrderInProcess>.Success(orderInProcess);
        }

        private async Task<Result<List<OrderEventStreamReadModel>>> GetProcessedEvents(int orderId)
        {
            var cycle = await _revertedOrderUoW.OrderEventStreams.GetLastProcessingCycle(orderId);
            if (cycle == null)
            {
                return Result<List<OrderEventStreamReadModel>>.Failure(new List<Error>{ new("RevertOrderCommandHandler.Events", $"Falha ao buscar os eventos de início e fim de processamento do pedido {orderId}.") }); 
            }

            var (start, end) = cycle.Value;
            var events = await _revertedOrderUoW.OrderEventStreams.GetEventsInWindow(orderId, start.Timestamp, end.Timestamp);
            if (events == null)
            {
                return Result<List<OrderEventStreamReadModel>>.Failure(new List<Error> { new("RevertOrderCommandHandler.Events", $"Falha ao buscar os eventos de processamento do pedido {orderId}.") });
            }

            var orderedEvents = events
                .OrderByDescending(e => e.Timestamp)
                .ThenByDescending(e => e.Version)
                .ToList();

            return Result<List<OrderEventStreamReadModel>>.Success(orderedEvents);
        }

        private async Task<Result<bool>> UpdateOriginalItems(OrderInProcess orderInProcess)
        {
            List<Error> updateErrors = new();
            foreach (var processedItem in orderInProcess.Items)
            {
                var getOrderItem = await _revertedOrderUoW.OrderItems.Get(processedItem.Id);
                if (getOrderItem.IsFailure)
                {
                    updateErrors.AddRange(getOrderItem.Errors!);
                    continue;
                }
                var orderItem = getOrderItem.GetValue();

                var getProduct = await _revertedOrderUoW.Products.Get(processedItem.ProductId);
                if (getProduct.IsFailure)
                {
                    updateErrors.AddRange(getProduct.Errors!);
                    continue;
                }
                var product = getProduct.GetValue();
                
                var discount = orderInProcess.UnAppliedDiscounts.FirstOrDefault(uad => uad.OwnerId == processedItem.Id && uad.DiscountScope == DiscountScope.Product);
                orderItem.Update(processedItem.Quantity, product.Price, discount is null ? null : discount.Id, true);

                var updateResult = await _revertedOrderUoW.OrderItems.Update(orderItem, true);
                if (updateResult.IsFailure)
                {
                    updateErrors.AddRange(updateResult.Errors!);
                    continue;
                }
            }

            if (updateErrors.Count != 0)
                return Result<bool>.Failure(updateErrors);

            return Result<bool>.Success(true);
        }

        private async Task<Result<bool>> RemoveUnusedItems(int orderId, OrderInProcess orderInProcess)
        {
            List<Error> deleteErrors = new();
    
            var getOrderItems = await _revertedOrderUoW.OrderItems.GetByOrderId(orderId);
            if (getOrderItems.IsFailure)
            {
                return Result<bool>.Failure(getOrderItems.Errors!);
            }
            var orderItems = getOrderItems.GetValue();

            var unusedItems = orderItems
                .Where(item => !orderInProcess.Items.Any(i => i.Id == item.Id))
                .ToList();
            foreach (var unusedItem in unusedItems)
            {
                var deleteResult = await _revertedOrderUoW.OrderItems.Delete(unusedItem.Id, true);
                if (deleteResult.IsFailure)
                {
                    deleteErrors.AddRange(deleteResult.Errors!);
                }
            }

            if (deleteErrors.Count != 0)
                return Result<bool>.Failure(deleteErrors);

            return Result<bool>.Success(true);
        }
    }
}

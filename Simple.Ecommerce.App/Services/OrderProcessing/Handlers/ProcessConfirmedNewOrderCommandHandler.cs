using Simple.Ecommerce.App.Interfaces.Services.OrderProcessing;
using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.CouponEntity;
using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain.Enums.OrderLock;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Exceptions.ResultException;
using Simple.Ecommerce.Domain.OrderProcessing.Commands;
using Simple.Ecommerce.Domain.OrderProcessing.Events.OrderEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.OrderProcessEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Models;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Handlers
{
    public class ProcessConfirmedNewOrderCommandHandler : IOrderProcessingCommandHandler<ProcessConfirmedNewOrderCommand, Result<bool>>
    {
        private readonly IConfirmOrderUnitOfWork _confirmOrderUoW;
        private readonly IOrderProcessingDispatcher _orderDispatcher;
        private readonly IOrderProcessingChain _confirmChain;

        public ProcessConfirmedNewOrderCommandHandler(
            IConfirmOrderUnitOfWork confirmOrderUoW, 
            IOrderProcessingDispatcher orderDispatcher, 
            IOrderProcessingChain confirmChain
        )
        {
            _confirmOrderUoW = confirmOrderUoW;
            _orderDispatcher = orderDispatcher;
            _confirmChain = confirmChain;
        }

        public async Task<Result<bool>> Handle(ProcessConfirmedNewOrderCommand command)
        {
            // Pegando os dados do command
            var order = command.Order;
            var userName = command.UserName;
            var orderCoupon = command.OrderCoupon;
            var orderDiscount = command.OrderDiscount;
            var completeOrderItems = command.CompleteOrderItems;

            Console.WriteLine($"\n[ProcessConfirmedNewOrderCommandHandler] Começando o processamento do pedido {order.Id}.");
            try
            {
                // Atualizando o estado do pedido para "Confirmed"
                order.UpdateStatus("Confirmed", OrderLock.Unlock, confirmation: true);
                if (order.Validate() is { IsFailure: true } cResult)
                {
                    throw new ResultException(cResult.Errors!);
                }

                // Criando o pedido em processamento    
                var orderInProcess = CreateOrderInProcess(order, orderCoupon, orderDiscount, completeOrderItems);

                // Criando e enviando o pedido de início do processamento do pedido
                Console.WriteLine($"[ProcessConfirmedNewOrderCommandHandler] Processando o pedido {order.Id} com {completeOrderItems.Count} itens diferentes.");
                await _orderDispatcher.Dispatch(new OrderProcessingStartedEvent(
                    order.Id,
                    order.UserId,
                    userName,
                    order.OrderType,
                    order.Address,
                    order.PaymentInformation,
                    order.OrderLock,
                    order.OrderDate!.Value,
                    order.Status,
                    orderInProcess.CurrentTotalPrice,
                    orderInProcess.Items.Select(item => new OrderItemEntry(
                        item.Id,
                        item.ProductId,
                        item.ProductName,
                        item.CurrentPrice,
                        item.Quantity
                    )).ToList()
                ));

                // Processando o pedido pela cadeia de resposabilidade de confirmação
                var processingResult = await _confirmChain.Process(orderInProcess);
                if (processingResult.IsFailure)
                {
                    throw new ResultException(processingResult.Errors!);
                }

                // Criando novos itens do para itens com tipos de descontos que necessitam de diferenciação entre o item com e sem o desconto
                var createResult = await CreateNewOrderItems(order.Id, orderInProcess);
                if (createResult.IsFailure)
                {
                    throw new ResultException(createResult.Errors!);
                }

                // Atualizando os itens existentes do pedido com os dados processados
                var orderItemsById = completeOrderItems.Select(o => o.Item1).ToList().ToDictionary(i => i.Id);
                var updateResult = await UpdateExistingItems(order.Id, orderInProcess, orderItemsById);
                if (updateResult.IsFailure)
                {
                    throw new ResultException(updateResult.Errors!);
                }

                // Enviando todos os eventos criados durante o processamento
                var uncommitedEvents = orderInProcess.GetUncommittedEvents();
                if (uncommitedEvents.Any())
                {
                    await _orderDispatcher.Dispatch(uncommitedEvents);
                }

                // Atualizando o status do pedido para "Processed"
                order.UpdateStatus("Processed", OrderLock.LockPrice, newTotalPrice: orderInProcess.CurrentTotalPrice);
                if (order.Validate() is { IsFailure: true } pResult)
                {
                    throw new ResultException(pResult.Errors!);
                }

                // Enviando o evento de pedido processado
                Console.WriteLine($"[ProcessConfirmedOrderCommandHandler] Pedido {order.Id} processado com sucesso. Total: {order.TotalPrice:C}.");
                await _orderDispatcher.Dispatch(new OrderProcessedEvent(order.Id, order.Status, order.TotalPrice!.Value, order.OrderLock));

                order.UpdateStatus("Pending Payment", order.OrderLock);
                if (order.Validate() is { IsFailure: true } ppResult)
                {
                    Console.WriteLine($"[ProcessConfirmedOrderCommandHandler] Falha ao atualizar o status pedido {order.Id}.");
                    throw new ResultException(ppResult.Errors!);
                }
                var updateOrderResult = await _confirmOrderUoW.Orders.Update(order, true);
                if (updateOrderResult.IsFailure)
                {
                    Console.WriteLine();
                    throw new ResultException(updateOrderResult.Errors!);
                }

                // Salvando as alterações em pedido e itens do pedido
                await _confirmOrderUoW.SaveChanges();

                return Result<bool>.Success(true);
            }
            catch (ResultException rex)
            {
                Console.WriteLine($"[ProcessConfirmedNewOrderCommandHandler] Falha ao processar o pedido {command.Order.Id}.");
                order.UpdateStatus("Failed Confirmed", order.OrderLock, false, 0);
                await _orderDispatcher.Dispatch(new OrderStatusChangedEvent(order.Id, order.Status));
                return Result<bool>.Failure(rex.Errors);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProcessConfirmedNewOrderCommandHandler] Falha ao processar o pedido {command.Order.Id}.");
                order.UpdateStatus("Failed Confirmed", order.OrderLock, false, 0);
                await _orderDispatcher.Dispatch(new OrderStatusChangedEvent(order.Id, order.Status));
                return Result<bool>.Failure(new List<Error>{ new("ProcessConfirmedNewOrderCommandHandler.Unknown", ex.Message) });
            }
        }

        private OrderInProcess CreateOrderInProcess(Order order, Coupon? orderCoupon, Discount? orderDiscount, List<(OrderItem orderItem, string productName, Coupon? orderItemCoupon, Discount? orderItemDiscount)> completeOrderItems)
        {
            var unAppliedDiscounts = new List<DiscountInProcess>();
            var orderItemsInProcess = new List<OrderItemInProcess>();

            if (orderDiscount is not null)
            {
                unAppliedDiscounts.Add(new DiscountInProcess(
                    orderDiscount.Id,
                    order.Id,
                    orderDiscount.Name,
                    orderDiscount.DiscountType,
                    orderDiscount.DiscountScope,
                    orderDiscount.DiscountValueType,
                    orderDiscount.Value,
                    orderDiscount.ValidFrom,
                    orderDiscount.ValidTo,
                    orderDiscount.IsActive,
                    orderCoupon is null 
                        ? null 
                        : new CouponInProcess(
                            orderCoupon.Id,
                            orderCoupon.DiscountId,
                            orderCoupon.Code,
                            orderCoupon.ExpirationAt,
                            orderCoupon.IsUsed
                        )));
            }

            foreach (var item in completeOrderItems)
            {
                var orderItem = item.orderItem;
                var productName = item.productName;
                var orderItemCoupon = item.orderItemCoupon;
                var orderItemDiscount = item.orderItemDiscount;
                orderItemsInProcess.Add(new OrderItemInProcess(
                    orderItem.Id,
                    orderItem.ProductId,
                    productName,
                    orderItem.Quantity,
                    orderItem.Price
                ));
                if (orderItemDiscount is not null)
                {
                    unAppliedDiscounts.Add(new DiscountInProcess(
                        orderItemDiscount.Id,
                        orderItem.Id,
                        orderItemDiscount.Name,
                        orderItemDiscount.DiscountType,
                        orderItemDiscount.DiscountScope,
                        orderItemDiscount.DiscountValueType,
                        orderItemDiscount.Value,
                        orderItemDiscount.ValidFrom,
                        orderItemDiscount.ValidTo,
                        orderItemDiscount.IsActive,
                        orderItemCoupon is null 
                            ? null 
                            : new CouponInProcess(
                                orderItemCoupon.Id,
                                orderItemCoupon.DiscountId,
                                orderItemCoupon.Code,
                                orderItemCoupon.ExpirationAt,
                                orderItemCoupon.IsUsed
                            )));
                }
            }

            var originalTotalPrice = completeOrderItems.Sum(item => item.orderItem.Quantity * item.orderItem.Price);
            return new OrderInProcess(
                order.Id,
                order.UserId,
                order.OrderType,
                order.Address,
                order.PaymentInformation,
                order.OrderLock,
                originalTotalPrice,
                orderItemsInProcess,
                unAppliedDiscounts,
                order.Status
            );
        }

        private async Task<Result<bool>> CreateNewOrderItems(int orderId, OrderInProcess orderInProcess)
        {
            var newOrderItems = new List<OrderItem>();

            if (orderInProcess.FreeItems.Count > 0)
            {
                foreach (var freeItem in orderInProcess.FreeItems)
                {
                    var instance = new OrderItem().Create(
                        0,
                        freeItem.CurrentPrice,
                        freeItem.Quantity,
                        freeItem.ProductId,
                        orderId,
                        freeItem.AppliedDiscount.CouponId,
                        freeItem.AppliedDiscount.DiscountId
                    );
                    if (instance.IsFailure)
                    {
                        throw new ResultException(instance.Errors!);
                    }
                    newOrderItems.Add(instance.GetValue());
                }
            }

            if (orderInProcess.Bundles.Count > 0)
            {
                foreach (var bundle in orderInProcess.Bundles)
                {
                    foreach (var item in bundle.Items)
                    {
                        var instance = new OrderItem().Create(
                            0,
                            item.CurrentPrice,
                            item.Quantity,
                            item.ProductId,
                            orderId,
                            item.AppliedDiscount.CouponId,
                            item.AppliedDiscount.DiscountId
                        );
                        if (instance.IsFailure)
                        {
                            throw new ResultException(instance.Errors!);
                        }
                        newOrderItems.Add(instance.GetValue());
                    }
                }
            }

            var newItemsErros = new List<Error>();
            foreach (var newItem in newOrderItems)
            {
                var createItemResult = await _confirmOrderUoW.OrderItems.Create(newItem, true);
                if (createItemResult.IsFailure)
                {
                    newItemsErros.AddRange(createItemResult.Errors!);
                }
            }

            if (newItemsErros.Count > 0)
                return Result<bool>.Failure(newItemsErros);
            return Result<bool>.Success(true);
        }

        private async Task<Result<bool>> UpdateExistingItems(int orderId, OrderInProcess orderInProcess, Dictionary<int, OrderItem> originalOrderItemsById)
        {
            var updatedItemsErros = new List<Error>();
            foreach (var processedItem in orderInProcess.Items)
            {
                if (!originalOrderItemsById.TryGetValue(processedItem.Id, out var originalItem))
                {
                    Console.WriteLine($"[ProcessConfirmedOrderCommandHandler] [WARN] OrderItem {processedItem.Id} não encontrado nos itens originais. Logo else foi criado durante o processamento.");
                    continue;
                }

                originalItem.Update(
                    processedItem.Quantity,
                    processedItem.CurrentPrice,
                    processedItem.AppliedDiscount is null ? null : processedItem.AppliedDiscount.CouponId,
                    processedItem.AppliedDiscount is null ? null : processedItem.AppliedDiscount.DiscountId,
                    true
                );
                if (originalItem.Validate() is { IsFailure: true } result)
                {
                    throw new ResultException(result.Errors!);
                }

                var updateResult = await _confirmOrderUoW.OrderItems.Update(originalItem, true);
                if (updateResult.IsFailure)
                {
                    updatedItemsErros.AddRange(updateResult.Errors!);
                    continue;
                }
            }

            if (updatedItemsErros.Count > 0)
                return Result<bool>.Failure(updatedItemsErros);
            return Result<bool>.Success(true);
        }
    }
}

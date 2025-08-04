using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.OrderProcessing;
using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;
using Simple.Ecommerce.Contracts.OrderContracts.Discounts;
using Simple.Ecommerce.Contracts.OrderItemContracts.Discounts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain.Enums.OrderLock;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Exceptions.ResultException;
using Simple.Ecommerce.Domain.OrderProcessing.Commands;
using Simple.Ecommerce.Domain.OrderProcessing.Events.OrderEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.OrderProcessEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Models;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Handlers
{
    public class ProcessConfirmedOrderCommandHandler : IOrderProcessingCommandHandler<ProcessConfirmedOrderCommand, Result<bool>>
    {
        private readonly IConfirmOrderUnitOfWork _confirmOrderUoW;
        private readonly IOrderProcessingDispatcher _orderDispatcher;
        private readonly IOrderProcessingChain _confirmChain;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public ProcessConfirmedOrderCommandHandler(
            IConfirmOrderUnitOfWork confirmOrderUoW,
            IOrderProcessingDispatcher orderDispatcher,
            IOrderProcessingChain confirmChain,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _confirmOrderUoW = confirmOrderUoW;
            _orderDispatcher = orderDispatcher;
            _confirmChain = confirmChain;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<bool>> Handle(ProcessConfirmedOrderCommand command)
        {
            Console.WriteLine($"\n[ProcessConfirmedOrderCommandHandler] Començando o processamento do pedido {command.OrderId}.");
            await _confirmOrderUoW.BeginTransaction();
            try
            {
                // Pegando dados do pedido
                var getOrder = await _confirmOrderUoW.Orders.Get(command.OrderId);
                if (getOrder.IsFailure)
                {
                    Console.WriteLine($"[ProcessConfirmedOrderCommandHandler] Falha ao pegar os dados do pedido {command.OrderId}.");
                    throw new ResultException(getOrder.Errors!);
                }
                var order = getOrder.GetValue();

                // Verificando a trava do pedido
                if (order.OrderLock is not OrderLock.Unlock)
                {
                    await _confirmOrderUoW.Rollback();
                    Console.WriteLine($"[ProcessConfirmedOrderCommandHandler] O pedido {command.OrderId} está bloqueado para processamento!");
                    return Result<bool>.Failure(new List<Error> { new("ProcessConfirmedOrderCommandHandler.Locked", $"O pedido {command.OrderId} está bloqueado para processamento!") });
                }

                // Pegando dados do usuário que fez o pedido
                var getUser = await _confirmOrderUoW.Users.Get(order.UserId);
                if (getUser.IsFailure)
                {
                    Console.WriteLine($"[ProcessConfirmedOrderCommandHandler] Falha ao pegar os dados do usuário {order.UserId}.");
                    throw new ResultException(getUser.Errors!);
                }

                // Atualizando o status do pedido para "Confirmed"
                order.UpdateStatus("Confirmed", OrderLock.Unlock, confirmation: true);
                if (order.Validate() is { IsFailure: true } cResult)
                {
                    throw new ResultException(cResult.Errors!);
                }

                // Pegando os dados do desconto do pedido se existir
                var getOrderDiscountDTO = await _confirmOrderUoW.Orders.GetDiscountDTO(command.OrderId);
                if (getOrderDiscountDTO.IsFailure)
                {
                    Console.WriteLine($"[ProcessConfirmedOrderCommandHandler] Falha ao pegar os dados do desconto do pedido {command.OrderId}.");
                    throw new ResultException(getOrderDiscountDTO.Errors!);
                }
                var orderDiscountDTO = getOrderDiscountDTO.GetValue();

                // Pegando os itens do pedido com os dados de seu desconto se existir
                var getOrderItemWithDiscountDTOs = await _confirmOrderUoW.OrderItems.ListByOrderIdOrderItemWithDiscountDTO(command.OrderId);
                if (getOrderItemWithDiscountDTOs.IsFailure)
                {
                    Console.WriteLine($"[ProcessConfirmedOrderCommandHandler] Falha ao pegar os itens do pedido {command.OrderId}.");
                    throw new ResultException(getOrderItemWithDiscountDTOs.Errors!);
                }
                var orderItemWithDiscounts = getOrderItemWithDiscountDTOs.GetValue();

                // Criando o pedido em processamento
                var originalItemsById = orderItemWithDiscounts.ToDictionary(item => item.Id);
                var orderInProcess = CreateOrderInProcess(order, orderDiscountDTO, orderItemWithDiscounts);

                // Enviando o evento de início do processamento do pedido
                Console.WriteLine($"[ProcessConfirmedOrderCommandHandler] Processando o pedido {command.OrderId} com {orderItemWithDiscounts.Count} itens diferentes.");
                await _orderDispatcher.Dispatch(new OrderProcessingStartedEvent(
                    order.Id,
                    order.UserId,
                    getUser.GetValue().Name,
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

                // Criando novos itens do pedido para itens com tipos de descontos que necessitam de diferenciação entre o item com e sem o desconto
                var createResult = await CreateNewOrderItems(order.Id, orderInProcess);
                if (createResult.IsFailure)
                {
                    Console.WriteLine($"[ProcessConfirmedOrderCommandHandler] Falha ao criar novos itens ao pedido {command.OrderId}.");
                    throw new ResultException(createResult.Errors!);
                }

                // Atualizando os itens existentes do pedido com os dados processados
                var updateResult = await UpdateExistingItems(order.Id, orderInProcess, originalItemsById);
                if (updateResult.IsFailure)
                {
                    Console.WriteLine($"[ProcessConfirmedOrderCommandHandler] Falha ao atualizar os itens do pedido {command.OrderId}.");
                    throw new ResultException(updateResult.Errors!);
                }

                // Despaichando os eventos não commitados do pedido em processamento
                var uncommittedEvents = orderInProcess.GetUncommittedEvents();
                if (uncommittedEvents.Any())
                {
                    await _orderDispatcher.Dispatch(uncommittedEvents);
                }

                // Atualizando o status do pedido para "Processed"
                order.UpdateStatus("Processed", OrderLock.LockPrice, newTotalPrice: orderInProcess.CurrentTotalPrice);
                if (order.Validate() is { IsFailure: true } pResult)
                {
                    Console.WriteLine($"[ProcessConfirmedOrderCommandHandler] Falha ao atualizar o status pedido {command.OrderId}.");
                    throw new ResultException(pResult.Errors!);
                }
                
                // Enviando o evento de pedido processado
                Console.WriteLine($"[ProcessConfirmedOrderCommandHandler] Pedido {command.OrderId} processado com sucesso. Total: {order.TotalPrice:C}.");
                await _orderDispatcher.Dispatch(new OrderProcessedEvent(order.Id, order.Status, order.TotalPrice!.Value, order.OrderLock));

                // Atualizando o status do pedido para Pagamento Pendente
                order.UpdateStatus("Pending Payment", order.OrderLock);
                if (order.Validate() is { IsFailure: true } ppResult)
                {
                    Console.WriteLine($"[ProcessConfirmedOrderCommandHandler] Falha ao atualizar o status pedido {command.OrderId}.");
                    throw new ResultException(ppResult.Errors!);
                }

                var updateOrderResult = await _confirmOrderUoW.Orders.Update(order, true);
                if (updateOrderResult.IsFailure)
                {
                    Console.WriteLine($"[ProcessConfirmedOrderCommandHandler] Falha ao atualizar o status pedido {command.OrderId}.");
                    throw new ResultException(updateOrderResult.Errors!);
                }

                // Salvando as alterações no pedido e nos itens do pedido
                await _confirmOrderUoW.Commit();
                if (_useCache.Use)
                {
                    _cacheHandler.SetItemStale<Order>();
                    _cacheHandler.SetItemStale<OrderItem>();
                }

                return Result<bool>.Success(true);
            }
            catch (ResultException rex)
            {
                await _confirmOrderUoW.Rollback();
                Console.WriteLine($"[ProcessConfirmedOrderCommandHandler] Falha ao processar o pedido {command.OrderId}.");
                var getOrder = await _confirmOrderUoW.Orders.Get(command.OrderId);
                if (getOrder.IsSuccess)
                {
                    var order = getOrder.GetValue();
                    order.UpdateStatus("Failed Confirmed", order.OrderLock, false, 0);
                    await _confirmOrderUoW.Orders.Update(order);
                    await _orderDispatcher.Dispatch(new OrderStatusChangedEvent(order.Id, order.Status));
                }
                return Result<bool>.Failure(rex.Errors);
            }
            catch (Exception ex)
            {
                await _confirmOrderUoW.Rollback();
                Console.WriteLine($"[ProcessConfirmedOrderCommandHandler] Falha ao processar o pedido {command.OrderId}.");
                var getOrder = await _confirmOrderUoW.Orders.Get(command.OrderId);
                if (getOrder.IsSuccess)
                {
                    var order = getOrder.GetValue();
                    order.UpdateStatus("Failed Confirmed", order.OrderLock, false, 0);
                    await _confirmOrderUoW.Orders.Update(order);
                    await _orderDispatcher.Dispatch(new OrderStatusChangedEvent(order.Id, order.Status));
                }
                return Result<bool>.Failure(new List<Error> { new("ProcessConfirmedOrderCommandHandler.Unknown", ex.Message) });
            }
        }

        private OrderInProcess CreateOrderInProcess(Order order, OrderDiscountDTO orderDiscountDTO, List<OrderItemWithDiscountDTO> orderItemsWithDiscount)
        {
            var unAppliedDiscounts = new List<DiscountInProcess>();
            var orderItemsInProcess = new List<OrderItemInProcess>();
            
            if (orderDiscountDTO.Discount is not null)
            {
                unAppliedDiscounts.Add(new DiscountInProcess(
                    orderDiscountDTO.Discount.Id,
                    order.Id,
                    orderDiscountDTO.Discount.Name,
                    orderDiscountDTO.Discount.DiscountType,
                    orderDiscountDTO.Discount.DiscountScope,
                    orderDiscountDTO.Discount.DiscountValueType,
                    orderDiscountDTO.Discount.Value,
                    orderDiscountDTO.Discount.ValidFrom,
                    orderDiscountDTO.Discount.ValidTo,
                    orderDiscountDTO.Discount.IsActive,
                    orderDiscountDTO.Discount.Coupon is null 
                        ? null 
                        : new CouponInProcess(
                            orderDiscountDTO.Discount.Coupon.Id,
                            orderDiscountDTO.Discount.Coupon.DiscountId,
                            orderDiscountDTO.Discount.Coupon.Code,
                            orderDiscountDTO.Discount.Coupon.ExpirationAt,
                            orderDiscountDTO.Discount.Coupon.IsUsed
                        )));
            }

            foreach (var item in orderItemsWithDiscount)
            {
                orderItemsInProcess.Add(new OrderItemInProcess(
                    item.Id,
                    item.ProductId,
                    item.ProductName,
                    item.Quantity,
                    item.Price
                ));
                if (item.Discount is not null)
                {
                    unAppliedDiscounts.Add(new DiscountInProcess(
                        item.Discount.Id,
                        item.Id,
                        item.Discount.Name,
                        item.Discount.DiscountType,
                        item.Discount.DiscountScope,
                        item.Discount.DiscountValueType,
                        item.Discount.Value,
                        item.Discount.ValidFrom,
                        item.Discount.ValidTo,
                        item.Discount.IsActive,
                        item.Discount.Coupon is null 
                            ? null 
                            : new CouponInProcess(
                                item.Discount.Coupon.Id,
                                item.Discount.Coupon.DiscountId,
                                item.Discount.Coupon.Code,
                                item.Discount.Coupon.ExpirationAt,
                                item.Discount.Coupon.IsUsed
                            )));
                }
            }

            var originalTotalPrice = orderItemsWithDiscount.Sum(item => item.Price * item.Quantity);

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

        private async Task<Result<bool>> CreateNewOrderItems(int id, OrderInProcess orderInProcess)
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
                        id,
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
                            id,
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

        private async Task<Result<bool>> UpdateExistingItems(int id, OrderInProcess orderInProcess, Dictionary<int, OrderItemWithDiscountDTO> originalItemsById)
        {
            var updatedItemsErros = new List<Error>();
            foreach (var processedItem in orderInProcess.Items)
            {
                if (!originalItemsById.TryGetValue(processedItem.Id, out var originalItem))
                {
                    Console.WriteLine($"[ProcessConfirmedOrderCommandHandler] [WARN] OrderItem {processedItem.Id} não encontrado nos itens originais. Logo foi criado no processamento.");
                    continue;
                }

                var updatedItem = new OrderItem().Create(
                    originalItem.Id,
                    processedItem.CurrentPrice,
                    processedItem.Quantity,
                    originalItem.ProductId,
                    id,
                    processedItem.AppliedDiscount is null ? null : processedItem.AppliedDiscount.CouponId,
                    processedItem.AppliedDiscount is null ? null : processedItem.AppliedDiscount.DiscountId
                );
                if (updatedItem.IsFailure)
                {
                    throw new ResultException(updatedItem.Errors!);
                }

                var updateResult = await _confirmOrderUoW.OrderItems.Update(updatedItem.GetValue(), true);
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

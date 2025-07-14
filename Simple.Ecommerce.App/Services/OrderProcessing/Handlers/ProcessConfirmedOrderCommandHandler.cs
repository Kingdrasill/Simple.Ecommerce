using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.OrderProcessing;
using Simple.Ecommerce.App.Interfaces.Services.UnityOfWork;
using Simple.Ecommerce.App.Services.OrderProcessing.ChainOfResponsibility;
using Simple.Ecommerce.Contracts.OrderContracts;
using Simple.Ecommerce.Contracts.OrderItemContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Exceptions.ResultException;
using Simple.Ecommerce.Domain.OrderProcessing.Commands;
using Simple.Ecommerce.Domain.OrderProcessing.Events.OrderEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Models;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Handlers
{
    public class ProcessConfirmedOrderCommandHandler : IOrderProcessingCommandHandler<ProcessConfirmedOrderCommand, Result<Order>>
    {
        private readonly IConfirmedOrderUnityOfWork _confirmedOrderUoW;
        private readonly IOrderProcessingDispatcher _orderDispatcher;
        private readonly IOrderProcessingChain _orderChain;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public ProcessConfirmedOrderCommandHandler(
            IConfirmedOrderUnityOfWork confirmedOrderUoW,
            IOrderProcessingDispatcher orderDispatcher,
            IOrderProcessingChain orderChain,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _confirmedOrderUoW = confirmedOrderUoW;
            _orderDispatcher = orderDispatcher;
            _orderChain = orderChain;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<Order>> Handle(ProcessConfirmedOrderCommand command)
        {
            Console.WriteLine($"\n[ProcessConfirmedOrderCommandHandler] Començando o processamento do pedido {command.OrderId}.");
            await _confirmedOrderUoW.BeginTransaction();
            try
            {
                // Pegando dados do pedido
                var getOrder = await _confirmedOrderUoW.Orders.Get(command.OrderId);
                if (getOrder.IsFailure)
                {
                    Console.WriteLine($"[ProcessConfirmedOrderCommandHandler] Falha ao pegar os dados do pedido {command.OrderId}.");
                    throw new ResultException(getOrder.Errors!);
                }
                var order = getOrder.GetValue();

                if (order.Status is not ("Created" or "Confirmed" or "Canceled" or "Failed"))
                {
                    Console.WriteLine($"[ProcessConfirmedOrderCommandHandler] O pedido {command.OrderId} já foi processado sucessivelmente anteriormente! Cancele ele para processar novamente!");
                    throw new ResultException(new Error("ProcessConfirmedOrderCommandHandler.AlreadyProcessed", $"O pedido { command.OrderId } já foi processado sucessivelmente anteriormente! Cancele ele para processar novamente!"));
                }

                order.UpdateStatus("Confirmed", true);

                // Pegando dados do usuário que fez o pedido
                var getUser = await _confirmedOrderUoW.Users.Get(order.UserId);
                if (getUser.IsFailure)
                {
                    Console.WriteLine($"[ProcessConfirmedOrderCommandHandler] Falha ao pegar os dados do usuário {order.UserId}.");
                    throw new ResultException(getUser.Errors!);
                }

                // Pegando os dados do desconto do pedido se existir
                var getOrderDiscountDTO = await _confirmedOrderUoW.Orders.GetDiscountDTOById(command.OrderId);
                if (getOrderDiscountDTO.IsFailure)
                {
                    Console.WriteLine($"[ProcessConfirmedOrderCommandHandler] Falha ao pegar os dados do desconto do pedido {command.OrderId}.");
                    throw new ResultException(getOrderDiscountDTO.Errors!);
                }
                var orderDiscountDTO = getOrderDiscountDTO.GetValue();

                // Pegando os itens do pedido com os dados de seu desconto se existir
                var getOrderItemsWithDiscountDTO = await _confirmedOrderUoW.OrderItems.GetOrderItemsWithDiscountDTO(command.OrderId);
                if (getOrderItemsWithDiscountDTO.IsFailure)
                {
                    Console.WriteLine($"[ProcessConfirmedOrderCommandHandler] Falha ao pegar os itens do pedido {command.OrderId}.");
                    throw new ResultException(getOrderItemsWithDiscountDTO.Errors!);
                }
                var orderItemsWithDiscount = getOrderItemsWithDiscountDTO.GetValue();

                // Criando o pedido em processamento
                var originalItemsById = orderItemsWithDiscount.ToDictionary(item => item.Id);
                var orderInProcess = CreateOrderInProcess(order, orderDiscountDTO, orderItemsWithDiscount);

                // Enviando o evento de início do processamento do pedido
                Console.WriteLine($"[ProcessConfirmedOrderCommandHandler] Processando o pedido {command.OrderId} com {orderItemsWithDiscount.Count} itens.");
                await _orderDispatcher.Dispatch(new OrderProcessingStartedEvent(
                    order.Id,
                    order.UserId,
                    getUser.GetValue().Name,
                    order.OrderType,
                    order.Address,
                    order.PaymentMethod!.Value,
                    order.CardInformation,
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

                // Processando o pedido com a cadeia de responsabilidade
                var processingResult = await _orderChain.Process(orderInProcess);
                if (processingResult.IsFailure)
                {
                    throw new ResultException(processingResult.Errors!);
                }

                // Criando novos itens do pedido para itens de graça e partes de pacote 
                var createResult = await CreateNewOrderItems(order.Id, orderInProcess);
                if (createResult.IsFailure)
                {
                    throw new ResultException(createResult.Errors!);
                }

                // Atualizando os itens existentes do pedido com os dados processados
                var updateResult = await UpdateExistingItems(order.Id, orderInProcess, originalItemsById);
                if (updateResult.IsFailure)
                {
                    throw new ResultException(updateResult.Errors!);
                }

                // Despaichando os eventos não commitados do pedido em processamento
                var uncommittedEvents = orderInProcess.GetUncommittedEvents();
                if (uncommittedEvents.Any())
                {
                    await _orderDispatcher.Dispatch(uncommittedEvents);
                }

                // Atualizando o status do pedido para Processado
                order.UpdateStatus("Processed", newTotalPrice: orderInProcess.CurrentTotalPrice);
                
                // Enviando o evento de pedido processado
                Console.WriteLine($"[ProcessConfirmedOrderCommandHandler] Pedido {command.OrderId} processado com sucesso. Total: {order.TotalPrice:C}.");
                await _orderDispatcher.Dispatch(new OrderProcessedEvent(order.Id, order.Status, order.TotalPrice!.Value));

                order.UpdateStatus("Pending Payment");
                var updateOrderResult = await _confirmedOrderUoW.Orders.Update(order, true);
                if (updateOrderResult.IsFailure)
                {
                    throw new ResultException(updateOrderResult.Errors!);
                }

                // Salvando as alterações no pedido e nos itens do pedido
                await _confirmedOrderUoW.Commit();
                if (_useCache.Use)
                {
                    _cacheHandler.SetItemStale<Order>();
                    _cacheHandler.SetItemStale<OrderItem>();
                }

                return Result<Order>.Success(order);
            }
            catch (ResultException rex)
            {
                await _confirmedOrderUoW.Rollback();

                Console.WriteLine($"[ProcessConfirmedOrderCommandHandler] Falha ao processar os dados do pedido {command.OrderId}.");
                var getOrder = await _confirmedOrderUoW.Orders.Get(command.OrderId);
                if (getOrder.IsSuccess)
                {
                    var order = getOrder.GetValue();
                    order.UpdateStatus("Failed", false, 0);
                    await _confirmedOrderUoW.Orders.Update(order);

                    await _orderDispatcher.Dispatch(new OrderStatusChangedEvent(order.Id, order.Status));
                }

                return Result<Order>.Failure(rex.Errors);
            }
            catch (Exception ex)
            {
                await _confirmedOrderUoW.Rollback();

                Console.WriteLine($"[ProcessConfirmedOrderCommandHandler] Falha ao processar os dados do pedido {command.OrderId}.");
                var getOrder = await _confirmedOrderUoW.Orders.Get(command.OrderId);
                if (getOrder.IsSuccess)
                {
                    var order = getOrder.GetValue();
                    order.UpdateStatus("Failed", false, 0);
                    await _confirmedOrderUoW.Orders.Update(order);

                    await _orderDispatcher.Dispatch(new OrderStatusChangedEvent(order.Id, order.Status));
                }

                return Result<Order>.Failure(new List<Error> { new("ProcessConfirmedOrderCommandHandler.Unknown", ex.Message) });
            }
        }

        private OrderInProcess CreateOrderInProcess(Order order, OrderDiscountDTO? orderDiscountDTO, List<OrderItemWithDiscountDTO> orderItemsWithDiscount)
        {
            var unAppliedDiscounts = new List<OrderDiscountInProcess>();
            var orderItemsInProcess = new List<OrderItemInProcess>();
            var originalItemsById = orderItemsWithDiscount.ToDictionary(item => item.Id);
            
            if (orderDiscountDTO is not null)
            {
                unAppliedDiscounts.Add(new OrderDiscountInProcess(
                    orderDiscountDTO.DiscountId,
                    order.Id,
                    orderDiscountDTO.DiscountName,
                    orderDiscountDTO.DiscountType,
                    orderDiscountDTO.DiscountScope,
                    orderDiscountDTO.DiscountValueType,
                    orderDiscountDTO.Value,
                    orderDiscountDTO.ValidFrom,
                    orderDiscountDTO.ValidTo,
                    orderDiscountDTO.IsActive
                ));
            }

            foreach (var item in orderItemsWithDiscount)
            {
                orderItemsInProcess.Add(new OrderItemInProcess(
                    item.Id,
                    item.ProductId,
                    item.ProductName,
                    item.Quantity,
                    item.UnitPrice
                ));
                if (item.Discount is not null)
                {
                    unAppliedDiscounts.Add(new OrderDiscountInProcess(
                        item.Discount.DiscountId,
                        item.Id,
                        item.Discount.DiscountName,
                        item.Discount.DiscountType,
                        item.Discount.DiscountScope,
                        item.Discount.DiscountValueType,
                        item.Discount.Value,
                        item.Discount.ValidFrom,
                        item.Discount.ValidTo,
                        item.Discount.IsActive
                    ));
                }
            }

            var originalTotalPrice = orderItemsWithDiscount.Sum(item => item.UnitPrice * item.Quantity);

            return new OrderInProcess(
                order.Id,
                order.UserId,
                order.OrderType,
                order.Address,
                order.PaymentMethod,
                order.CardInformation,
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
                var createItemResult = await _confirmedOrderUoW.OrderItems.Create(newItem, true);
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
                    processedItem.AppliedDiscount is null ? null : processedItem.AppliedDiscount.DiscountId
                );
                if (updatedItem.IsFailure)
                {
                    throw new ResultException(updatedItem.Errors!);
                }

                var updateResult = await _confirmedOrderUoW.OrderItems.Update(updatedItem.GetValue(), true);
                if (updateResult.IsFailure)
                {
                    updatedItemsErros.AddRange(updateResult.Errors!);
                }
            }

            if (updatedItemsErros.Count > 0)
                return Result<bool>.Failure(updatedItemsErros);

            return Result<bool>.Success(true);
        }
    }
}

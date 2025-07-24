using Simple.Ecommerce.App.Interfaces.ReadData;
using Simple.Ecommerce.Domain.Interfaces.OrderProcessingEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ItemBOGOEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ItemBundleEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ItemSimpleEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ItemTieredEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.OrderDiscountEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.OrderEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ShippingEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.TaxEvent;
using Simple.Ecommerce.Domain.OrderProcessing.ReadModels;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Projectors
{
    public class UserOrderHistoryProjector :
        // Confirmação
        IOrderProcessingEventHandler<OrderProcessingStartedEvent>,
        IOrderProcessingEventHandler<OrderStatusChangedEvent>,
        IOrderProcessingEventHandler<ShippingFeeAppliedEvent>,
        IOrderProcessingEventHandler<SimpleItemDiscountAppliedEvent>,
        IOrderProcessingEventHandler<TieredItemDiscountAppliedEvent>,
        IOrderProcessingEventHandler<BOGOItemDiscountAppliedEvent>,
        IOrderProcessingEventHandler<BundleDiscountAppliedEvent>,
        IOrderProcessingEventHandler<OrderDiscountAppliedEvent>,
        IOrderProcessingEventHandler<TaxAppliedEvent>,
        IOrderProcessingEventHandler<OrderProcessedEvent>,
        // Reversão
        IOrderProcessingEventHandler<OrderRevertingStartedEvent>,
        IOrderProcessingEventHandler<TaxRevertedEvent>,
        IOrderProcessingEventHandler<OrderDiscountRevertedEvent>,
        IOrderProcessingEventHandler<BundleDiscountRevertEvent>,
        IOrderProcessingEventHandler<BOGOItemDiscountRevertEvent>,
        IOrderProcessingEventHandler<TieredItemDiscountRevertEvent>,
        IOrderProcessingEventHandler<SimpleItemDiscountRevertEvent>,
        IOrderProcessingEventHandler<ShippingFeeRevertedEvent>,
        IOrderProcessingEventHandler<OrderRevertedEvent>
    {
        private readonly IUserOrderHistoryReadModelRepository _userOrderHistoryReadModelRepository;

        public UserOrderHistoryProjector(
            IUserOrderHistoryReadModelRepository userOrderHistoryReadModelRepository
        )
        {
            _userOrderHistoryReadModelRepository = userOrderHistoryReadModelRepository;
        }

        // Confirmação
        public async Task Handle(OrderProcessingStartedEvent @event)
        {
            var readModel = await _userOrderHistoryReadModelRepository.GetByUserId(@event.UserId);
            if (readModel != null)
            {
                var orderEntry = readModel.Orders.FirstOrDefault(o => o.OrderId == @event.AggregateId);
                if (orderEntry != null)
                {
                    orderEntry.OrderDate = @event.Timestamp;
                    orderEntry.Status = @event.Status;
                    orderEntry.TotalAmount = @event.InitialTotal;
                    orderEntry.ItemsCount = @event.Items.Sum(item => item.Quantity);
                }
                else
                {
                    readModel.Orders.Add(new UserOrderHistoryEntry
                    {
                        OrderId = @event.AggregateId,
                        OrderDate = @event.Timestamp,
                        Status = @event.Status,
                        TotalAmount = @event.InitialTotal,
                        ItemsCount = @event.Items.Sum(item => item.Quantity)
                    });
                }
            }
            else
            {
                readModel = new UserOrderHistoryReadModel
                {
                    UserId = @event.UserId,
                    UserName = @event.UserName,
                    Orders = new List<UserOrderHistoryEntry>
                    {
                        new UserOrderHistoryEntry
                        {
                            OrderId = @event.AggregateId,
                            OrderDate = @event.Timestamp,
                            Status = @event.Status,
                            TotalAmount = @event.InitialTotal,
                            ItemsCount = @event.Items.Sum(item => item.Quantity)
                        }
                    }
                };
            }
            await _userOrderHistoryReadModelRepository.Upsert(readModel);
        }

        public async Task Handle(OrderStatusChangedEvent @event)
        {
            var readModel = await _userOrderHistoryReadModelRepository.GetByOrderId(@event.AggregateId);
            if (readModel != null)
            {
                var orderEntry = readModel.Orders.FirstOrDefault(o => o.OrderId == @event.AggregateId);
                if (orderEntry != null)
                {
                    orderEntry.Status = @event.NewStatus;
                    await _userOrderHistoryReadModelRepository.Upsert(readModel);
                }
            }
        }

        private async Task HandleGenericEvent(int orderId, decimal currentTotal)
        {
            var readModel = await _userOrderHistoryReadModelRepository.GetByOrderId(orderId);
            if (readModel != null)
            {
                var orderEntry = readModel.Orders.FirstOrDefault(o => o.OrderId == orderId);
                if (orderEntry != null)
                {
                    orderEntry.TotalAmount = currentTotal;
                    await _userOrderHistoryReadModelRepository.Upsert(readModel);
                }
            }
        }

        public async Task Handle(ShippingFeeAppliedEvent @event) => await HandleGenericEvent(@event.AggregateId, @event.CurrentTotal);
        public async Task Handle(SimpleItemDiscountAppliedEvent @event) => await HandleGenericEvent(@event.AggregateId, @event.CurrentTotal);
        public async Task Handle(TieredItemDiscountAppliedEvent @event) => await HandleGenericEvent(@event.AggregateId, @event.CurrentTotal);
        public async Task Handle(BOGOItemDiscountAppliedEvent @event) => await HandleGenericEvent(@event.AggregateId, @event.CurrentTotal);
        public async Task Handle(BundleDiscountAppliedEvent @event) => await HandleGenericEvent(@event.AggregateId, @event.CurrentTotal);
        public async Task Handle(OrderDiscountAppliedEvent @event) => await HandleGenericEvent(@event.AggregateId, @event.CurrentTotal);
        public async Task Handle(TaxAppliedEvent @event) => await HandleGenericEvent(@event.AggregateId, @event.CurrentTotal);
        
        public async Task Handle(OrderProcessedEvent @event)
        {
            var readModel = await _userOrderHistoryReadModelRepository.GetByOrderId(@event.AggregateId);
            if (readModel != null)
            {
                var orderEntry = readModel.Orders.FirstOrDefault(o => o.OrderId == @event.AggregateId);
                if (orderEntry != null)
                {
                    orderEntry.TotalAmount = @event.FinalTotal;
                    orderEntry.Status = @event.Status;
                    await _userOrderHistoryReadModelRepository.Upsert(readModel);
                }
            }
        }

        // Reversão
        public async Task Handle(OrderRevertingStartedEvent @event)
        {
            var readModel = await _userOrderHistoryReadModelRepository.GetByUserId(@event.UserId);
            if (readModel != null)
            {
                var orderEntry = readModel.Orders.FirstOrDefault(o => o.OrderId == @event.AggregateId);
                if (orderEntry != null)
                {
                    orderEntry.Status = @event.Status;
                }
                await _userOrderHistoryReadModelRepository.Upsert(readModel);
            }
        }

        public async Task Handle(TaxRevertedEvent @event) => await HandleGenericEvent(@event.AggregateId, @event.CurrentTotal);
        public async Task Handle(OrderDiscountRevertedEvent @event) => await HandleGenericEvent(@event.AggregateId, @event.CurrentTotal);
        public async Task Handle(BundleDiscountRevertEvent @event) => await HandleGenericEvent(@event.AggregateId, @event.CurrentTotal);
        public async Task Handle(BOGOItemDiscountRevertEvent @event) => await HandleGenericEvent(@event.AggregateId, @event.CurrentTotal);
        public async Task Handle(TieredItemDiscountRevertEvent @event) => await HandleGenericEvent(@event.AggregateId, @event.CurrentTotal);
        public async Task Handle(SimpleItemDiscountRevertEvent @event) => await HandleGenericEvent(@event.AggregateId, @event.CurrentTotal);
        public async Task Handle(ShippingFeeRevertedEvent @event) => await HandleGenericEvent(@event.AggregateId, @event.CurrentTotal);

        public async Task Handle(OrderRevertedEvent @event)
        {
            var readModel = await _userOrderHistoryReadModelRepository.GetByOrderId(@event.AggregateId);
            if (readModel != null)
            {
                var orderEntry = readModel.Orders.FirstOrDefault(o => o.OrderId == @event.AggregateId);
                if (orderEntry != null)
                {
                    orderEntry.TotalAmount = @event.OriginalTotal;
                    orderEntry.Status = @event.Status;
                    await _userOrderHistoryReadModelRepository.Upsert(readModel);
                }
            }
        }
    }
}

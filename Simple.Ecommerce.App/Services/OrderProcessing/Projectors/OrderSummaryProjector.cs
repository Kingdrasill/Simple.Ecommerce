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
    public class OrderSummaryProjector :
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
        private readonly IOrderSummaryReadModelRepository _orderSummaryReadModelRepository;

        public OrderSummaryProjector(
            IOrderSummaryReadModelRepository orderSummaryReadModelRepository
        )
        {
            _orderSummaryReadModelRepository = orderSummaryReadModelRepository;
        }

        // Confirmação
        public async Task Handle(OrderProcessingStartedEvent @event)
        {
            var orderSummary = new OrderSummaryReadModel
            {
                OrderId = @event.AggregateId,
                UserId = @event.UserId,
                OrderDate = @event.OrderDate,
                Status = @event.Status,
                TotalAmount = @event.InitialTotal,
                ItemsCount = @event.Items.Sum(item => item.Quantity)
            };
            await _orderSummaryReadModelRepository.Upsert(orderSummary);
        }

        public async Task Handle(OrderStatusChangedEvent @event)
        {
            var orderSummary = await _orderSummaryReadModelRepository.GetById(@event.AggregateId);
            if (orderSummary != null)
            {
                orderSummary.Status = @event.NewStatus;
                await _orderSummaryReadModelRepository.Upsert(orderSummary);
            }
        }

        private async Task UpdateTotalAmount(int aggregateId, decimal newTotal)
        {
            var orderSummary = await _orderSummaryReadModelRepository.GetById(aggregateId);
            if (orderSummary != null)
            {
                orderSummary.TotalAmount = newTotal;
                await _orderSummaryReadModelRepository.Upsert(orderSummary);
            }
        }

        public async Task Handle(ShippingFeeAppliedEvent @event) => await UpdateTotalAmount(@event.AggregateId, @event.CurrentTotal);
        public async Task Handle(SimpleItemDiscountAppliedEvent @event) => await UpdateTotalAmount(@event.AggregateId, @event.CurrentTotal);
        public async Task Handle(TieredItemDiscountAppliedEvent @event) => await UpdateTotalAmount(@event.AggregateId, @event.CurrentTotal);
        public async Task Handle(BOGOItemDiscountAppliedEvent @event) => await UpdateTotalAmount(@event.AggregateId, @event.CurrentTotal);
        public async Task Handle(BundleDiscountAppliedEvent @event) => await UpdateTotalAmount(@event.AggregateId, @event.CurrentTotal);
        public async Task Handle(OrderDiscountAppliedEvent @event) => await UpdateTotalAmount(@event.AggregateId, @event.CurrentTotal);
        public async Task Handle(TaxAppliedEvent @event) => await UpdateTotalAmount(@event.AggregateId, @event.CurrentTotal);
        
        public async Task Handle(OrderProcessedEvent @event)
        {
            var orderSummary = await _orderSummaryReadModelRepository.GetById(@event.AggregateId);
            if (orderSummary != null)
            {
                orderSummary.Status = @event.Status;
                orderSummary.TotalAmount = @event.FinalTotal;
                await _orderSummaryReadModelRepository.Upsert(orderSummary);
            }
        }

        // Reversão
        public async Task Handle(OrderRevertingStartedEvent @event)
        {
            int count = @event.Items.Sum(item => item.Quantity) + @event.FreeItems.Sum(item => item.Quantity) + @event.Bundles.Sum(bundle => bundle.BundleItems.Sum(item => item.Quantity));
            var orderSummary = new OrderSummaryReadModel
            {
                OrderId = @event.AggregateId,
                UserId = @event.UserId,
                OrderDate = @event.OrderDate,
                Status = @event.Status,
                TotalAmount = @event.FinalTotal,
                ItemsCount = count
            };
            await _orderSummaryReadModelRepository.Upsert(orderSummary);
        }

        public async Task Handle(TaxRevertedEvent @event) => await UpdateTotalAmount(@event.AggregateId, @event.CurrentTotal);
        public async Task Handle(OrderDiscountRevertedEvent @event) => await UpdateTotalAmount(@event.AggregateId, @event.CurrentTotal);
        public async Task Handle(BundleDiscountRevertEvent @event) => await UpdateTotalAmount(@event.AggregateId, @event.CurrentTotal);
        public async Task Handle(BOGOItemDiscountRevertEvent @event) => await UpdateTotalAmount(@event.AggregateId, @event.CurrentTotal);
        public async Task Handle(TieredItemDiscountRevertEvent @event) => await UpdateTotalAmount(@event.AggregateId, @event.CurrentTotal);
        public async Task Handle(SimpleItemDiscountRevertEvent @event) => await UpdateTotalAmount(@event.AggregateId, @event.CurrentTotal);
        public async Task Handle(ShippingFeeRevertedEvent @event) => await UpdateTotalAmount(@event.AggregateId, @event.CurrentTotal);

        public async Task Handle(OrderRevertedEvent @event)
        {
            var orderSummary = await _orderSummaryReadModelRepository.GetById(@event.AggregateId);
            if (orderSummary != null)
            {
                orderSummary.Status = @event.Status;
                orderSummary.TotalAmount = @event.OriginalTotal;
                await _orderSummaryReadModelRepository.Upsert(orderSummary);
            }
        }
    }
}

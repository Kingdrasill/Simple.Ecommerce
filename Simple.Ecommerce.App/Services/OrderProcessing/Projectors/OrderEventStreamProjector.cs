using MongoDB.Bson;
using Simple.Ecommerce.App.Interfaces.ReadData;
using Simple.Ecommerce.Domain.Interfaces.OrderProcessingEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ItemBOGOEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ItemBundleEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ItemSimpleEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ItemTieredEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.OrderDiscountEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.OrderEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.OrderProcessEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.OrderRevertEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ShippingEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.TaxEvent;
using Simple.Ecommerce.Domain.OrderProcessing.ReadModels;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Projectors
{
    public class OrderEventStreamProjector :
        // Confirmação
        IOrderProcessingEventHandler<OrderProcessingStartedEvent>,
        IOrderProcessingEventHandler<OrderStatusChangedEvent>,
        IOrderProcessingEventHandler<ShippingFeeAppliedEvent>,
        IOrderProcessingEventHandler<SimpleItemDiscountAppliedEvent>,
        IOrderProcessingEventHandler<TieredItemDiscountAppliedEvent>,
        IOrderProcessingEventHandler<BOGODiscountAppliedEvent>,
        IOrderProcessingEventHandler<BundleDiscountAppliedEvent>,
        IOrderProcessingEventHandler<OrderDiscountAppliedEvent>,
        IOrderProcessingEventHandler<TaxAppliedEvent>,
        IOrderProcessingEventHandler<OrderProcessedEvent>,
        // Reversão
        IOrderProcessingEventHandler<OrderRevertingStartedEvent>,
        IOrderProcessingEventHandler<TaxRevertedEvent>,
        IOrderProcessingEventHandler<OrderDiscountRevertedEvent>,
        IOrderProcessingEventHandler<BundleDiscountRevertEvent>,
        IOrderProcessingEventHandler<BOGODiscountRevertEvent>,
        IOrderProcessingEventHandler<TieredItemDiscountRevertEvent>,
        IOrderProcessingEventHandler<SimpleItemDiscountRevertEvent>,
        IOrderProcessingEventHandler<ShippingFeeRevertedEvent>,
        IOrderProcessingEventHandler<OrderRevertedEvent>
    {
        private readonly IOrderEventStreamReadModelRepository _orderEventStreamReadModelRepository;

        public OrderEventStreamProjector(
            IOrderEventStreamReadModelRepository orderEventStreamReadModelRepository
        )
        {
            _orderEventStreamReadModelRepository = orderEventStreamReadModelRepository;
        }

        private async Task HandleGenericEvent<TEvent>(TEvent @event) where TEvent : IOrderProcessingEvent
        {
            int nextVersion = await _orderEventStreamReadModelRepository.GetLastVersionForOrder(@event.AggregateId) + 1;

            var eventDataBson = @event.ToBsonDocument();

            var readModel = new OrderEventStreamReadModel(
                @event.EventId,
                @event.AggregateId,
                @event.EventType,
                @event.Timestamp,
                eventDataBson,
                nextVersion
            );

            await _orderEventStreamReadModelRepository.InsertOne(readModel);
        }

        // Confirmação
        public async Task Handle(OrderProcessingStartedEvent @event) => await HandleGenericEvent(@event);
        public async Task Handle(OrderStatusChangedEvent @event) => await HandleGenericEvent(@event);
        public async Task Handle(ShippingFeeAppliedEvent @event) => await HandleGenericEvent(@event);
        public async Task Handle(SimpleItemDiscountAppliedEvent @event) => await HandleGenericEvent(@event);
        public async Task Handle(TieredItemDiscountAppliedEvent @event) => await HandleGenericEvent(@event);
        public async Task Handle(BOGODiscountAppliedEvent @event) => await HandleGenericEvent(@event);
        public async Task Handle(BundleDiscountAppliedEvent @event) => await HandleGenericEvent(@event);
        public async Task Handle(OrderDiscountAppliedEvent @event) => await HandleGenericEvent(@event);
        public async Task Handle(TaxAppliedEvent @event) => await HandleGenericEvent(@event);
        public async Task Handle(OrderProcessedEvent @event) => await HandleGenericEvent(@event);

        // Reversão
        public async Task Handle(OrderRevertingStartedEvent @event) => await HandleGenericEvent(@event);
        public async Task Handle(TaxRevertedEvent @event) => await HandleGenericEvent(@event);
        public async Task Handle(OrderDiscountRevertedEvent @event) => await HandleGenericEvent(@event);
        public async Task Handle(BundleDiscountRevertEvent @event) => await HandleGenericEvent(@event);
        public async Task Handle(BOGODiscountRevertEvent @event) => await HandleGenericEvent(@event);
        public async Task Handle(TieredItemDiscountRevertEvent @event) => await HandleGenericEvent(@event);
        public async Task Handle(SimpleItemDiscountRevertEvent @event) => await HandleGenericEvent(@event);
        public async Task Handle(ShippingFeeRevertedEvent @event) => await HandleGenericEvent(@event);
        public async Task Handle(OrderRevertedEvent @event) => await HandleGenericEvent(@event);
    }
}

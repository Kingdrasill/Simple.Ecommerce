using MongoDB.Bson;
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
    public class OrderEventStreamProjector :
        IOrderProcessingEventHandler<OrderProcessingStartedEvent>,
        IOrderProcessingEventHandler<OrderStatusChangedEvent>,
        IOrderProcessingEventHandler<ShippingFeeAppliedEvent>,
        IOrderProcessingEventHandler<SimpleItemDiscountAppliedEvent>,
        IOrderProcessingEventHandler<TieredItemDiscountAppliedEvent>,
        IOrderProcessingEventHandler<BOGOItemDiscountAppliedEvent>,
        IOrderProcessingEventHandler<BundleDiscountAppliedEvent>,
        IOrderProcessingEventHandler<OrderDiscountAppliedEvent>,
        IOrderProcessingEventHandler<TaxAppliedEvent>,
        IOrderProcessingEventHandler<OrderProcessedEvent>
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

        public async Task Handle(OrderProcessingStartedEvent @event) => await HandleGenericEvent(@event);
        public async Task Handle(OrderStatusChangedEvent @event) => await HandleGenericEvent(@event);
        public async Task Handle(ShippingFeeAppliedEvent @event) => await HandleGenericEvent(@event);
        public async Task Handle(SimpleItemDiscountAppliedEvent @event) => await HandleGenericEvent(@event);
        public async Task Handle(TieredItemDiscountAppliedEvent @event) => await HandleGenericEvent(@event);
        public async Task Handle(BOGOItemDiscountAppliedEvent @event) => await HandleGenericEvent(@event);
        public async Task Handle(BundleDiscountAppliedEvent @event) => await HandleGenericEvent(@event);
        public async Task Handle(OrderDiscountAppliedEvent @event) => await HandleGenericEvent(@event);
        public async Task Handle(TaxAppliedEvent @event) => await HandleGenericEvent(@event);
        public async Task Handle(OrderProcessedEvent @event) => await HandleGenericEvent(@event);
    }
}

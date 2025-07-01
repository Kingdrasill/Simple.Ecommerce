using Simple.Ecommerce.App.Interfaces.ReadData;
using Simple.Ecommerce.Domain.Events.OrderEvent;
using Simple.Ecommerce.Domain.Interfaces.OrderEvent;
using Simple.Ecommerce.Domain.Objects;
using Simple.Ecommerce.Domain.ReadModels;

namespace Simple.Ecommerce.App.Handlers.OrderEvent
{
    public class OrderSummaryProjectionHandler : 
        IOrderEventHandler<OrderPlacedEvent>,
        IOrderEventHandler<OrderDeliveredEvent>,
        IOrderEventHandler<OrderCanceledEvent>
    {
        private readonly IOrderSummaryReadRepository _repository;

        public OrderSummaryProjectionHandler(
            IOrderSummaryReadRepository repository
        )
        {
            _repository = repository;
        }

        public async Task Handle(OrderPlacedEvent @event)
        {
            var order = new OrderSummaryReadModel
            {
                OrderId = @event.OrderId.ToString(),
                UserId = @event.UserId.ToString(),
                OrderDate = @event.OrderDate,
                Status = "Placed",
                TotalPrice = @event.TotalPrice,
                Items = @event.Items.Select(i => new OrderItemEntry
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            await _repository.Save(order);
        }

        public async Task Handle(OrderDeliveredEvent @event)
        {
            var order = await _repository.GetByOrderId(@event.OrderId.ToString());
            if (order == null) return;

            order.Status = "Delivered";
            await _repository.Save(order);
        }

        public async Task Handle(OrderCanceledEvent @event)
        {
            var order = await _repository.GetByOrderId(@event.OrderId.ToString());
            if (order == null) return;

            order.Status = "Canceled";
            await _repository.Save(order);
        }
    }
}

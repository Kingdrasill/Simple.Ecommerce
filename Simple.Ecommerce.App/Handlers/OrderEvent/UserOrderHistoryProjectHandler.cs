using Simple.Ecommerce.App.Interfaces.ReadData;
using Simple.Ecommerce.Domain.Events.OrderEvent;
using Simple.Ecommerce.Domain.Interfaces.OrderEvent;
using Simple.Ecommerce.Domain.ReadModels;
using Simple.Ecommerce.Domain.ValueObjects.UserOrderObject;

namespace Simple.Ecommerce.App.Handlers.OrderEvent
{
    public class UserOrderHistoryProjectHandler : 
        IOrderEventHandler<OrderPlacedEvent>,
        IOrderEventHandler<OrderDeliveredEvent>,
        IOrderEventHandler<OrderCanceledEvent>
    {
        private readonly IUserOrderHistoryReadRepository _repository;

        public UserOrderHistoryProjectHandler(
            IUserOrderHistoryReadRepository repository
        )
        {
            _repository = repository;
        }

        public async Task Handle(OrderPlacedEvent @event)
        {
            var history = await _repository.GetByUserId(@event.UserId.ToString())
                    ?? new UserOrderHistoryReadModel()
                    {
                        UserId = @event.UserId.ToString(),
                        Orders = new List<UserOrderEntry>()
                    };

            history.Orders.Add(new UserOrderEntry
            {
                OrderId = @event.OrderId,
                OrderDate = @event.OrderDate,
                Status = "Placed",
                TotalPrice = @event.TotalPrice
            });

            await _repository.Save(history);
        }

        public async Task Handle(OrderDeliveredEvent @event)
        {
            var history = await _repository.GetByUserId(@event.UserId.ToString())
                    ?? new UserOrderHistoryReadModel()
                    {
                        UserId = @event.UserId.ToString(),
                        Orders = new List<UserOrderEntry>()
                    };

            var orderHistory = history.Orders.FirstOrDefault(o => o.OrderId == @event.OrderId);

            if (orderHistory is null)
                return;

            orderHistory.Status = "Delivered";
            orderHistory.OrderDate = @event.DeliveredAt;

            await _repository.Save(history);
        }

        public async Task Handle(OrderCanceledEvent @event)
        {
            var history = await _repository.GetByUserId(@event.UserId.ToString())
                    ?? new UserOrderHistoryReadModel()
                    {
                        UserId = @event.UserId.ToString(),
                        Orders = new List<UserOrderEntry>()
                    };

            var orderHistory = history.Orders.FirstOrDefault(o => o.OrderId == @event.OrderId);

            if (orderHistory is null)
                return;

            orderHistory.Status = "Canceled";
            orderHistory.OrderDate = @event.CanceledAt;

            await _repository.Save(history);
        }
    }
}

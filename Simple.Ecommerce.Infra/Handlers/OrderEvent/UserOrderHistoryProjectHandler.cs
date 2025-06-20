using Simple.Ecommerce.App.Interfaces.ReadData;
using Simple.Ecommerce.Domain.Events.OrderEvent;
using Simple.Ecommerce.Domain.Interfaces.OrderEvent;
using Simple.Ecommerce.Domain.ReadModels;
using Simple.Ecommerce.Domain.ValueObjects.UserOrderObject;

namespace Simple.Ecommerce.Infra.Handlers.OrderEvent
{
    public class UserOrderHistoryProjectHandler : IOrderEventHandler<OrderPlacedEvent>
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
    }
}

using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Command;
using Simple.Ecommerce.App.Interfaces.Services.Dispatcher;
using Simple.Ecommerce.App.Interfaces.Services.Patterns.Processor;
using Simple.Ecommerce.App.Services.Command;

namespace Simple.Ecommerce.App.Handlers.OrderEvent
{
    public class PlaceOrderCommandHandler : ICommandHandler<PlaceOrderCommand>
    {
        private readonly List<IOrderProcessor> _processors;
        private readonly IOrderEventBus _orderEventBus;
        private readonly IOrderRepository _orderRepository;

        public PlaceOrderCommandHandler(
            List<IOrderProcessor> processors, 
            IOrderEventBus orderEventBus, 
            IOrderRepository orderRepository
        )
        {
            _processors = processors;
            _orderEventBus = orderEventBus;
            _orderRepository = orderRepository;
        }

        public Task Handle(PlaceOrderCommand command)
        {
            return Task.CompletedTask;
        }
    }
}

using Simple.Ecommerce.App.Interfaces.Services.Dispatcher;
using Simple.Ecommerce.Domain.Interfaces.OrderEvent;
using Microsoft.Extensions.DependencyInjection;

namespace Simple.Ecommerce.App.Services.Dispatcher
{
    public class OrderEventBus : IOrderEventBus
    {
        private readonly IServiceProvider _serviceProvider;

        public OrderEventBus(
            IServiceProvider serviceProvider
        )
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Publish<TEvent>(TEvent @event) where TEvent : class
        {
            var handlers = _serviceProvider.GetServices<IOrderEventHandler<TEvent>>();

            foreach (var handler in handlers)
            {
                await handler.Handle(@event);
            }
        }
    }
}

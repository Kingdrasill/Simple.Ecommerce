using Microsoft.Extensions.DependencyInjection;
using Simple.Ecommerce.App.Interfaces.Services.OrderProcessing;
using Simple.Ecommerce.Domain.Interfaces.OrderProcessingEvent;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Dispatcher
{
    public class OrderProcessingDispatcher : IOrderProcessingDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public OrderProcessingDispatcher(
            IServiceProvider serviceProvider
        )
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Dispatch(IOrderProcessingEvent @event)
        {
            var eventType = @event.GetType();

            var handlerType = typeof(IOrderProcessingEventHandler<>).MakeGenericType(eventType);

            var handlers = _serviceProvider.GetServices(handlerType);

            foreach (dynamic handler in handlers)
            {
                await handler.Handle((dynamic)@event);
            }
        }

        public async Task Dispatch(IEnumerable<IOrderProcessingEvent> events)
        {
            foreach (var @event in events)
            {
                await Dispatch(@event);
            }
        }
    }
}

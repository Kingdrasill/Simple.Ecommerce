using Simple.Ecommerce.App.Interfaces.Services.Dispatcher;
using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;
using Microsoft.Extensions.DependencyInjection;

namespace Simple.Ecommerce.Infra.Services.EventDispatcher
{
    public class DeleteEventDispatcher : IDeleteEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public DeleteEventDispatcher(
            IServiceProvider serviceProvider
        )
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Dispatch(IEnumerable<IDeleteEvent> domainEvents)
        {
            foreach (var domainEvent in domainEvents)
            {
                var handlerType = typeof(IDeleteEventHandler<>).MakeGenericType(domainEvent.GetType());
                var handlers = _serviceProvider.GetServices(handlerType);

                foreach (var handler in handlers)
                {
                    await ((Task)handlerType
                        .GetMethod("Handle")!
                        .Invoke(handler, new object[] { domainEvent })!);
                }
            }
        }
    }
}

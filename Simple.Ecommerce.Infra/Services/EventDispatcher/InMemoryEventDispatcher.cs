using ImageFile.Library.Core.Events;

namespace Simple.Ecommerce.Infra.Services.ImageEventDispatcher
{
    public class InMemoryEventDispatcher : IEventDispatcher
    {
        private readonly Dictionary<Type, List<Func<IDomainEvent, Task>>> _handlers = new();

        public void RegisterHandler<TEvent>(Func<TEvent, Task> handler) where TEvent : IDomainEvent
        {
            var eventType = typeof(TEvent);
            if (!_handlers.ContainsKey(eventType))
                _handlers[eventType] = new List<Func<IDomainEvent, Task>>();

            _handlers[eventType].Add(e => handler((TEvent)e));
        }

        public async Task DispatchAsync<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent
        {
            var eventType = typeof(TEvent);
            if (_handlers.TryGetValue(eventType, out var handlers))
            {
                foreach(var handler in handlers)
                {
                    await handler(domainEvent);
                }
            }
        }
    }
}

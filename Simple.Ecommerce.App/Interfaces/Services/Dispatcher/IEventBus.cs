namespace Simple.Ecommerce.App.Interfaces.Services.Dispatcher
{
    public interface IEventBus
    {
        Task Publish<TEvent>(TEvent @event) where TEvent : class;
    }
}

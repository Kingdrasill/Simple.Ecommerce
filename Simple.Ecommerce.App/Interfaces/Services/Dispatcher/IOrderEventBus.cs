namespace Simple.Ecommerce.App.Interfaces.Services.Dispatcher
{
    public interface IOrderEventBus
    {
        Task Publish<TEvent>(TEvent @event) where TEvent : class;
    }
}

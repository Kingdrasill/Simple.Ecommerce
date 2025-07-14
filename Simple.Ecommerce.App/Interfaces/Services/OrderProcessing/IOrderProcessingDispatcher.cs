using Simple.Ecommerce.Domain.Interfaces.OrderProcessingEvent;

namespace Simple.Ecommerce.App.Interfaces.Services.OrderProcessing
{
    public interface IOrderProcessingDispatcher
    {
        Task Dispatch(IOrderProcessingEvent @event);
        Task Dispatch(IEnumerable<IOrderProcessingEvent> events);
    }
}

using Simple.Ecommerce.Domain.OrderProcessing.Models;

namespace Simple.Ecommerce.App.Interfaces.Services.OrderProcessing
{
    public interface IOrderProcessingHandler
    {
        void SetNext(IOrderProcessingHandler handler);
        Task Handle(OrderInProcess orderInProcess, bool skip = false);
    }
}

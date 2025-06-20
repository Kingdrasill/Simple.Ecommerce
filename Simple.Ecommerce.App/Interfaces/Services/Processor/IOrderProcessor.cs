using Simple.Ecommerce.Domain.Entities.OrderEntity;

namespace Simple.Ecommerce.App.Interfaces.Services.Processor
{
    public interface IOrderProcessor
    {
        Task Process(Order order);
    }
}

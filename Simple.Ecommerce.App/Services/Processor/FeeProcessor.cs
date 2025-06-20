using Simple.Ecommerce.App.Interfaces.Services.Processor;
using Simple.Ecommerce.Domain.Entities.OrderEntity;

namespace Simple.Ecommerce.App.Services.Processor
{
    public class FeeProcessor : IOrderProcessor
    {
        public Task Process(Order order)
        {
            return Task.CompletedTask;
        }
    }
}

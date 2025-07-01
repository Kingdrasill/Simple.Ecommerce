using Simple.Ecommerce.App.Interfaces.Services.Patterns.Processor;
using Simple.Ecommerce.Domain.Entities.OrderEntity;

namespace Simple.Ecommerce.App.Services.Processor
{
    public class TaxProcessor : IOrderProcessor
    {
        public Task Process(Order order)
        {
            return Task.CompletedTask;
        }
    }
}

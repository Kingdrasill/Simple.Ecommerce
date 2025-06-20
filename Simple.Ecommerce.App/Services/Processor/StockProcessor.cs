using Simple.Ecommerce.App.Interfaces.Services.Dispatcher;
using Simple.Ecommerce.App.Interfaces.Services.Processor;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Events.OrderEvent;

namespace Simple.Ecommerce.App.Services.Processor
{
    public class StockProcessor : IOrderProcessor
    {
        private readonly IOrderEventBus _orderEventBus;

        public StockProcessor(
            IOrderEventBus orderEventBus
        )
        {
            _orderEventBus = orderEventBus;
        }

        public async Task Process(Order order)
        {
            foreach (var item in order.OrderItems)
            {
                var stockEvent = new StockMovedEvent
                {
                    ProductId = item.ProductId,
                    QuantityMoved = item.Quantity,
                    Reason = "Order Placed",
                    OccuredAt = DateTime.UtcNow
                };

                await _orderEventBus.Publish(stockEvent);
            }
        }
    }
}

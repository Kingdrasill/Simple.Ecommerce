using Simple.Ecommerce.App.Interfaces.ReadData;
using Simple.Ecommerce.Domain.Events.OrderEvent;
using Simple.Ecommerce.Domain.Interfaces.OrderEvent;
using Simple.Ecommerce.Domain.ReadModels;

namespace Simple.Ecommerce.Infra.Handlers.OrderEvent
{
    public class StockProjectionHandler : IOrderEventHandler<StockMovedEvent>
    {
        private readonly IStockReadRepository _repository;

        public StockProjectionHandler(
            IStockReadRepository repository
        )
        {
            _repository = repository;
        }

        public async Task Handle(StockMovedEvent @event)
        {
            var stock = await _repository.GetByProductId(@event.ProductId)
                         ?? new StockReadModel { ProductId = @event.ProductId };

            stock.QuantityAvailable -= @event.QuantityMoved;
            stock.LastUpdated = DateTime.UtcNow;

            await _repository.Save(stock);
        }
    }

}

using Simple.Ecommerce.App.Interfaces.ReadData;
using Simple.Ecommerce.Domain.Interfaces.OrderProcessingEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.StockEvent;
using Simple.Ecommerce.Domain.OrderProcessing.ReadModels;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Projectors
{
    public class StockMovementProjector :
        IOrderProcessingEventHandler<StockReservedEvent>
    {
        private readonly IStockMovementReadModelRepository _stockMovementReadModelRepository;

        public StockMovementProjector(
            IStockMovementReadModelRepository stockMovementReadModelRepository
        )
        {
            _stockMovementReadModelRepository = stockMovementReadModelRepository;
        }

        public async Task Handle(StockReservedEvent @event)
        {
            var readModel = new StockMovementReadModel
            {
                MovementId = Guid.NewGuid(),
                ProductId = @event.ProductId,
                QuantityChanged = -@event.Quantity,
                MovementType = "Reserved",
                OrderId = @event.AggregateId,
                Timestamp = @event.Timestamp
            };

            await _stockMovementReadModelRepository.InsertOne(readModel);
        }
    }
}

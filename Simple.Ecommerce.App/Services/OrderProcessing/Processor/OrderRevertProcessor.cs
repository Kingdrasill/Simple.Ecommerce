using Simple.Ecommerce.App.Interfaces.Services.OrderProcessing;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.OrderProcessing.Models;
using Simple.Ecommerce.Domain.OrderProcessing.ReadModels;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Processor
{
    public class OrderRevertProcessor
    {
        private readonly Dictionary<string, IOrderRevertingHandler> _handlers;

        public OrderRevertProcessor(IEnumerable<IOrderRevertingHandler> handlers)
        {
            _handlers = handlers.ToDictionary(h => h.EventType);
        }

        public async Task<Result<bool>> Revert(OrderInProcess order, List<OrderEventStreamReadModel> events)
        {
            foreach (var evt in events)
            {
                if (_handlers.TryGetValue(evt.EventType, out var handler))
                {
                    var result = await handler.Revert(order, evt.EventData);
                    if (result.IsFailure)
                        return result;
                }
            }

            return Result<bool>.Success(true);
        }
    }
}

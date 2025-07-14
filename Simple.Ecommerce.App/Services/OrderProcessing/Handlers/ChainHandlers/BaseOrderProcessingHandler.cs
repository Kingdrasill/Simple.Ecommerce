using Simple.Ecommerce.App.Interfaces.Services.OrderProcessing;
using Simple.Ecommerce.Domain.OrderProcessing.Models;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Handlers.ChainHandlers
{
    public abstract class BaseOrderProcessingHandler : IOrderProcessingHandler
    {
        private IOrderProcessingHandler _nexHandler;

        protected BaseOrderProcessingHandler() { }

        public void SetNext(IOrderProcessingHandler handler)
        {
            _nexHandler = handler;
        }

        public virtual async Task Handle(OrderInProcess orderInProcess, bool skipDiscounts = false)
        {
            if (_nexHandler != null)
            {
                await _nexHandler.Handle(orderInProcess, skipDiscounts);
            }
        }
    }
}

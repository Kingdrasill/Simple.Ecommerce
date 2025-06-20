using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Domain.Entities.OrderDiscountEnity;
using Simple.Ecommerce.Domain.Events.DeletedEvent;
using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;
using Microsoft.EntityFrameworkCore;

namespace Simple.Ecommerce.Infra.Handlers.DeletedEvent
{
    public class OrderDeletedEventHandler : IDeleteEventHandler<OrderDeletedEvent>
    {
        private readonly TesteDbContext _context;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public OrderDeletedEventHandler(
            TesteDbContext context, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _context = context;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task Handle(OrderDeletedEvent domainEvent)
        {
            var orderDiscounts = await _context.OrderDiscounts
                .Where(od => od.OrderId == domainEvent.OrderId)
                .ToListAsync();

            foreach (var orderDiscount in orderDiscounts)
            {
                orderDiscount.MarkAsDeleted(raiseEvent: false);
            }

            if (_useCache.Use)
            {
                if (orderDiscounts.Any())
                    _cacheHandler.SetItemStale<OrderDiscount>();
            }    
        }
    }
}

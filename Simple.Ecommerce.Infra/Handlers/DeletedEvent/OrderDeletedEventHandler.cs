using Microsoft.EntityFrameworkCore;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain.Events.DeletedEvent;
using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

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
            var orderItems = await _context.OrderItems
                .Where(od => od.OrderId == domainEvent.OrderId)
                .ToListAsync();

            foreach (var orderItem in orderItems)
            {
                orderItem.MarkAsDeleted(raiseEvent: false);
            }

            if (_useCache.Use)
            {
                if (orderItems.Any())
                    _cacheHandler.SetItemStale<OrderItem>();
            }    
        }
    }
}

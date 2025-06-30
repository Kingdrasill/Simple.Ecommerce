using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Domain.Entities.LoginEntity;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Entities.ReviewEntity;
using Simple.Ecommerce.Domain.Entities.UserAddressEntity;
using Simple.Ecommerce.Domain.Events.DeletedEvent;
using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;
using Microsoft.EntityFrameworkCore;
using Simple.Ecommerce.Domain.Entities.UserCardEntity;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.Infra.Handlers.DeletedEvent
{
    public class UserDeletedEventHandler : IDeleteEventHandler<UserDeletedEvent>
    {
        private readonly TesteDbContext _context;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public UserDeletedEventHandler(
            TesteDbContext context, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _context = context;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task Handle(UserDeletedEvent domainEvent)
        {
            var logins = await _context.Logins
                .Where(l => l.UserId == domainEvent.UserId)
                .ToListAsync();

            foreach (var login in logins)
            {
                login.MarkAsDeleted(raiseEvent: true);
            }

            var orders = await _context.Orders
                .Where(o => o.UserId == domainEvent.UserId)
                .ToListAsync();

            foreach (var order in orders)
            {
                order.MarkAsDeleted(raiseEvent: true);
            }

            var userAddresses = await _context.UserAddresses
                .Where(ua => ua.UserId == domainEvent.UserId)
                .ToListAsync();

            foreach (var userAddress in userAddresses)
            {
                userAddress.MarkAsDeleted(raiseEvent: false);
            }

            var userCards = await _context.UserCards
                .Where(uc => uc.UserId == domainEvent.UserId)
                .ToListAsync();

            foreach (var userCard in userCards)
            {
                userCard.MarkAsDeleted(raiseEvent: false);
            }

            var reviews = await _context.Reviews
                .Where(r => r.UserId == domainEvent.UserId)
                .ToListAsync();

            foreach (var review in reviews)
            {
                review.MarkAsDeleted(raiseEvent: false);
            }

            if (_useCache.Use)
            {
                if (logins.Any())
                    _cacheHandler.SetItemStale<Login>();
                if (orders.Any())
                    _cacheHandler.SetItemStale<Order>();
                if (userAddresses.Any())
                    _cacheHandler.SetItemStale<UserAddress>();
                if (userCards.Any())
                    _cacheHandler.SetItemStale<UserCard>();
                if (reviews.Any())
                    _cacheHandler.SetItemStale<Review>();
            }
        }
    }
}

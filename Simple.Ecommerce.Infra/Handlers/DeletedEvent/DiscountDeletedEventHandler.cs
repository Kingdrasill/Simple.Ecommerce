using Microsoft.EntityFrameworkCore;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Domain.Entities.DiscountBundleItemEntity;
using Simple.Ecommerce.Domain.Entities.DiscountTierEntity;
using Simple.Ecommerce.Domain.Entities.ProductDiscountEntity;
using Simple.Ecommerce.Domain.EntityDeletionEvents;
using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.Infra.Handlers.DeletedEvent
{
    public class DiscountDeletedEventHandler : IDeleteEventHandler<DiscountDeletedEvent>
    {
        private readonly TesteDbContext _context;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public DiscountDeletedEventHandler(
            TesteDbContext context, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _context = context;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task Handle(DiscountDeletedEvent domainEvent)
        {
            var coupons = await _context.Coupons
                .Where(c => c.DiscountId == domainEvent.DiscountId)
                .ToListAsync();

            foreach (var coupon in coupons)
            {
                coupon.MarkAsDeleted(raiseEvent: false);
            }

            var discountBundleItems = await _context.DiscountBundleItems
                .Where(dbi => dbi.DiscountId == domainEvent.DiscountId)
                .ToListAsync();

            foreach (var discountBundleItem in discountBundleItems)
            {
                discountBundleItem.MarkAsDeleted(raiseEvent: false);
            }

            var discountTiers = await _context.DiscountTiers
                .Where(dt => dt.DiscountId == domainEvent.DiscountId)
                .ToListAsync();

            foreach (var discountTier in discountTiers)
            {
                discountTier.MarkAsDeleted(raiseEvent: false);
            }

            var productDiscounts = await _context.ProductDiscounts
                .Where(pd => pd.DiscountId == domainEvent.DiscountId)
                .ToListAsync();

            foreach (var productDiscount in productDiscounts)
            {
                productDiscount.MarkAsDeleted(raiseEvent: false);
            }

            if (_useCache.Use)
            {
                if (discountBundleItems.Any())
                    _cacheHandler.SetItemStale<DiscountBundleItem>();
                if (discountTiers.Any())
                    _cacheHandler.SetItemStale<DiscountTier>();
                if (productDiscounts.Any())
                    _cacheHandler.SetItemStale<ProductDiscount>();
            }
        }
    }
}

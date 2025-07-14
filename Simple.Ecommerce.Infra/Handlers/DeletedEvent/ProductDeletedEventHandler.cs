using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain.Entities.ProductCategoryEntity;
using Simple.Ecommerce.Domain.Entities.ProductDiscountEntity;
using Simple.Ecommerce.Domain.Entities.ProductPhotoEntity;
using Simple.Ecommerce.Domain.Entities.ReviewEntity;
using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;
using Microsoft.EntityFrameworkCore;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;
using Simple.Ecommerce.Domain.EntityDeletionEvents;

namespace Simple.Ecommerce.Infra.Handlers.DeletedEvent
{
    public class ProductDeletedEventHandler : IDeleteEventHandler<ProductDeletedEvent>
    {
        private readonly TesteDbContext _context;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public ProductDeletedEventHandler(
            TesteDbContext context, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _context = context;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task Handle(ProductDeletedEvent domainEvent)
        {
            var cartItems = await _context.OrderItems
                .Where(ci => ci.ProductId == domainEvent.ProductId)
                .ToListAsync();

            foreach (var cartItem in cartItems)
            {
                cartItem.MarkAsDeleted(raiseEvent: false);
            }

            var productCategories = await _context.ProductCategories
                .Where(pc => pc.ProductId == domainEvent.ProductId)
                .ToListAsync();

            foreach (var productCategory in productCategories)
            {
                productCategory.MarkAsDeleted(raiseEvent: false);
            }

            var productDiscounts = await _context.ProductDiscounts
                .Where(pd => pd.ProductId == domainEvent.ProductId)
                .ToListAsync();

            foreach (var productDiscount in productDiscounts)
            {
                productDiscount.MarkAsDeleted(raiseEvent: false);
            }

            var productPhotos = await _context.ProductPhotos
                .Where(pp => pp.ProductId == domainEvent.ProductId)
                .ToListAsync();

            foreach (var productPhoto in productPhotos)
            {
                productPhoto.MarkAsDeleted(raiseEvent: false);
            }

            var reviews = await _context.Reviews
                .Where(r => r.ProductId  == domainEvent.ProductId)
                .ToListAsync();

            foreach (var review in reviews)
            {
                review.MarkAsDeleted(raiseEvent: false);
            }

            if (_useCache.Use)
            {
                if (cartItems.Any())
                    _cacheHandler.SetItemStale<OrderItem>();
                if (productCategories.Any())
                    _cacheHandler.SetItemStale<ProductCategory>();
                if (productDiscounts.Any())
                    _cacheHandler.SetItemStale<ProductDiscount>();
                if (productPhotos.Any())
                    _cacheHandler.SetItemStale<ProductPhoto>();
                if (reviews.Any())
                    _cacheHandler.SetItemStale<Review>();
            }
        }
    }
}

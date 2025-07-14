using Microsoft.EntityFrameworkCore;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Domain.Entities.ProductCategoryEntity;
using Simple.Ecommerce.Domain.EntityDeletionEvents;
using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.Infra.Handlers.DeletedEvent
{
    public class CategoryDeletedEventHandler : IDeleteEventHandler<CategoryDeletedEvent>
    {
        private readonly TesteDbContext _context;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public CategoryDeletedEventHandler(
            TesteDbContext context,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _context = context;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task Handle(CategoryDeletedEvent domainEvent)
        {
            var productCategories = await _context.ProductCategories
                .Where(pc => pc.CategoryId == domainEvent.CategoryId)
                .ToListAsync();

            foreach (var productCategory in productCategories)
            {
                productCategory.MarkAsDeleted(raiseEvent: false);
            }

            if (_useCache.Use)
                _cacheHandler.SetItemStale<ProductCategory>();
        }
    }
}

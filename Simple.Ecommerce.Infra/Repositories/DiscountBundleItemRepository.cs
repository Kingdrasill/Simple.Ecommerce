using Microsoft.EntityFrameworkCore;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.Domain.Entities.DiscountBundleItemEntity;
using Simple.Ecommerce.Domain.Objects;
using Simple.Ecommerce.Infra.Interfaces.Generic;

namespace Simple.Ecommerce.Infra.Repositories
{
    public class DiscountBundleItemRepository : IDiscountBundleItemRepository
    {
        private readonly TesteDbContext _context;
        private readonly IGenericCreateRepository<DiscountBundleItem> _createRepository;
        private readonly IGenericDeleteRepository<DiscountBundleItem> _deleteRepository;
        private readonly IGenericGetRepository<DiscountBundleItem> _getRepository;
        private readonly IGenericListRepository<DiscountBundleItem> _listRepository;
        private readonly IGenericUpdateRepository<DiscountBundleItem> _updateRepository;

        public DiscountBundleItemRepository(
            TesteDbContext context,
            IGenericCreateRepository<DiscountBundleItem> createRepository,
            IGenericDeleteRepository<DiscountBundleItem> deleteRepository,
            IGenericGetRepository<DiscountBundleItem> getRepository,
            IGenericListRepository<DiscountBundleItem> listRepository,
            IGenericUpdateRepository<DiscountBundleItem> updateRepository
        )
        {
            _context = context;
            _createRepository = createRepository;
            _deleteRepository = deleteRepository;
            _getRepository = getRepository;
            _listRepository = listRepository;
            _updateRepository = updateRepository;
        }

        public async Task<Result<DiscountBundleItem>> Create(DiscountBundleItem entity)
        {
            return await _createRepository.Create(_context, entity);
        }

        public async Task<Result<bool>> Delete(int id)
        {
            return await _deleteRepository.Delete(_context, id);
        }

        public async Task<Result<DiscountBundleItem>> Get(int id, bool NoTracking = true)
        {
            return await _getRepository.Get(_context, id, NoTracking);
        }

        public async Task<Result<List<DiscountBundleItem>>> GetByDiscountId(int discountId)
        {
            var discountBundleItems = await _context.DiscountBundleItems
                .Where(dbi => dbi.DiscountId == discountId && !dbi.Deleted)
                .ToListAsync();

            return Result<List<DiscountBundleItem>>.Success(discountBundleItems);
        }

        public async Task<Result<List<DiscountBundleItem>>> GetByProductId(int productId)
        {
            var discountBundleItems = await (
                    from dbi in _context.DiscountBundleItems
                    join d in _context.Discounts on dbi.DiscountId equals d.Id into discountJoin
                    from discount in discountJoin.DefaultIfEmpty()
                    where
                            discount.IsActive
                        &&  discount.ValidFrom <= DateTime.UtcNow && discount.ValidTo >= DateTime.UtcNow
                        && !dbi.Deleted
                    select dbi
                ).ToListAsync();

            return Result<List<DiscountBundleItem>>.Success(discountBundleItems);
        }

        public async Task<Result<List<int>>> GetProductIdsByDiscountId(int discountId)
        {
            var discountBundleIds = await _context.DiscountBundleItems
                .Where(dbi => dbi.DiscountId == discountId && !dbi.Deleted)
                .Select(dbi => dbi.ProductId)
                .ToListAsync();

            return Result<List<int>>.Success(discountBundleIds);
        }

        public async Task<Result<List<DiscountBundleItem>>> List()
        {
            return await _listRepository.List(_context);
        }

        public async Task<Result<DiscountBundleItem>> Update(DiscountBundleItem entity)
        {
            return await _updateRepository.Update(_context, entity);
        }
    }
}

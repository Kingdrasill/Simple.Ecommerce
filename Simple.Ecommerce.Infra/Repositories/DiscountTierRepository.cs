using Microsoft.EntityFrameworkCore;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.DiscountTierEntity;
using Simple.Ecommerce.Infra.Interfaces.Generic;

namespace Simple.Ecommerce.Infra.Repositories
{
    public class DiscountTierRepository : IDiscountTierRepository
    {
        private readonly TesteDbContext _context;
        private readonly IGenericCreateRepository<DiscountTier> _createRepository;
        private readonly IGenericDeleteRepository<DiscountTier> _deleteRepository;
        private readonly IGenericGetRepository<DiscountTier> _getRepository;
        private readonly IGenericListRepository<DiscountTier> _listRepository;
        private readonly IGenericUpdateRepository<DiscountTier> _updateRepository;

        public DiscountTierRepository(
            TesteDbContext context, 
            IGenericCreateRepository<DiscountTier> createRepository, 
            IGenericDeleteRepository<DiscountTier> deleteRepository, 
            IGenericGetRepository<DiscountTier> getRepository, 
            IGenericListRepository<DiscountTier> listRepository, 
            IGenericUpdateRepository<DiscountTier> updateRepository
        )
        {
            _context = context;
            _createRepository = createRepository;
            _deleteRepository = deleteRepository;
            _getRepository = getRepository;
            _listRepository = listRepository;
            _updateRepository = updateRepository;
        }

        public async Task<Result<DiscountTier>> Create(DiscountTier entity, bool skipSave = false)
        {
            return await _createRepository.Create(_context, entity);
        }

        public async Task<Result<bool>> Delete(int id, bool skipSave = false)
        {
            return await _deleteRepository.Delete(_context, id);
        }

        public async Task<Result<DiscountTier>> Get(int id, bool NoTracking = true)
        {
            return await _getRepository.Get(_context, id, NoTracking);
        }

        public async Task<Result<List<DiscountTier>>> GetByDiscountId(int discountId)
        {
            var discountTiers = await _context.DiscountTiers
                                                .Where(dt => dt.DiscountId == discountId && !dt.Deleted)
                                                .ToListAsync();

            return Result<List<DiscountTier>>.Success(discountTiers);
        }

        public async Task<Result<List<DiscountTier>>> List()
        {
            return await _listRepository.List(_context);
        }

        public async Task<Result<DiscountTier>> Update(DiscountTier entity, bool skipSave = false)
        {
            return await _updateRepository.Update(_context, entity);
        }
    }
}

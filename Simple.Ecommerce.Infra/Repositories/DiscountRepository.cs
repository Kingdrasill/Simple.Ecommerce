using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Infra.Interfaces.Generic;

namespace Simple.Ecommerce.Infra.Repositories
{
    internal class DiscountRepository : IDiscountRepository
    {
        private readonly TesteDbContext _context;
        private readonly IGenericCreateRepository<Discount> _createRepository;
        private readonly IGenericDeleteRepository<Discount> _deleteRepository;
        private readonly IGenericGetRepository<Discount> _getRepository;
        private readonly IGenericListRepository<Discount> _listRepository;
        private readonly IGenericUpdateRepository<Discount> _updateRepository;

        public DiscountRepository(
            TesteDbContext context, 
            IGenericCreateRepository<Discount> createRepository, 
            IGenericDeleteRepository<Discount> deleteRepository, 
            IGenericGetRepository<Discount> getRepository, 
            IGenericListRepository<Discount> listRepository, 
            IGenericUpdateRepository<Discount> updateRepository
        )
        {
            _context = context;
            _createRepository = createRepository;
            _deleteRepository = deleteRepository;
            _getRepository = getRepository;
            _listRepository = listRepository;
            _updateRepository = updateRepository;
        }

        public async Task<Result<Discount>> Create(Discount entity)
        {
            return await _createRepository.Create(_context, entity);
        }

        public async Task<Result<bool>> Delete(int id)
        {
            return await _deleteRepository.Delete(_context, id);
        }

        public async Task<Result<Discount>> Get(int id, bool NoTracking = true)
        {
            return await _getRepository.Get(_context, id, NoTracking);
        }

        public async Task<Result<List<Discount>>> List()
        {
            return await _listRepository.List(_context);
        }

        public async Task<Result<Discount>> Update(Discount entity)
        {
            return await _updateRepository.Update(_context, entity);
        }
    }
}

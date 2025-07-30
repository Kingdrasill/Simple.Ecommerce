using Microsoft.EntityFrameworkCore;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Infra.Interfaces.Generic;

namespace Simple.Ecommerce.Infra.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly TesteDbContext _context;
        private readonly IGenericCreateRepository<Discount> _createRepository;
        private readonly IGenericDeleteRepository<Discount> _deleteRepository;
        private readonly IGenericDetachRepository<Discount> _detachRepository;
        private readonly IGenericGetRepository<Discount> _getRepository;
        private readonly IGenericListRepository<Discount> _listRepository;
        private readonly IGenericUpdateRepository<Discount> _updateRepository;

        public DiscountRepository(
            TesteDbContext context, 
            IGenericCreateRepository<Discount> createRepository, 
            IGenericDeleteRepository<Discount> deleteRepository, 
            IGenericDetachRepository<Discount> detachRepository,
            IGenericGetRepository<Discount> getRepository, 
            IGenericListRepository<Discount> listRepository, 
            IGenericUpdateRepository<Discount> updateRepository
        )
        {
            _context = context;
            _createRepository = createRepository;
            _deleteRepository = deleteRepository;
            _detachRepository = detachRepository;
            _getRepository = getRepository;
            _listRepository = listRepository;
            _updateRepository = updateRepository;
        }

        public async Task<Result<Discount>> Create(Discount entity, bool skipSave = false)
        {
            return await _createRepository.Create(_context, entity);
        }

        public async Task<Result<bool>> Delete(int id, bool skipSave = false)
        {
            return await _deleteRepository.Delete(_context, id);
        }

        public void Detach(Discount entity)
        {
            _detachRepository.Detach(_context, entity);
        }

        public async Task<Result<Discount>> Get(int id, bool NoTracking = true)
        {
            return await _getRepository.Get(_context, id, NoTracking);
        }

        public async Task<Result<List<Discount>>> GetByDiscountIds(List<int> ids)
        {
            var discounts = await _context.Discounts
                .Where(d => ids.Contains(d.Id) && !d.Deleted)
                .ToListAsync();

            if (discounts.Count != ids.Count)
            {
                return Result<List<Discount>>.Failure(new List<Error>{ new("DiscountRepository.NotFound", "Um ou mais descontons não foram encontrados!") });
            }

            return Result<List<Discount>>.Success(discounts);
        }

        public async Task<Result<List<Discount>>> GetDiscountsByIds(List<int> ids)
        {
            return Result<List<Discount>>.Success(await _context.Discounts
                .Where(d => ids.Contains(d.Id) && !d.Deleted)
                .ToListAsync());
        }

        public async Task<Result<List<Discount>>> List()
        {
            return await _listRepository.List(_context);
        }

        public async Task<Result<Discount>> Update(Discount entity, bool skipSave = false)
        {
            return await _updateRepository.Update(_context, entity);
        }
    }
}

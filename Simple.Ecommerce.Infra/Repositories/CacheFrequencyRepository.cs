using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.Domain.Entities.FrequencyEntity;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.Infra.Interfaces.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Simple.Ecommerce.Infra.Repositories
{
    public class CacheFrequencyRepository : ICacheFrequencyRepository
    {
        private readonly TesteDbContext _context;
        private readonly IGenericCreateRepository<CacheFrequency> _createRepository;
        private readonly IGenericUpdateRepository<CacheFrequency> _updateRepository;
        private readonly IGenericGetRepository<CacheFrequency> _getRepository;
        private readonly IGenericListRepository<CacheFrequency> _listRepository;

        public CacheFrequencyRepository(
            TesteDbContext context, 
            IGenericCreateRepository<CacheFrequency> createRepository, 
            IGenericUpdateRepository<CacheFrequency> updateRepository,
            IGenericGetRepository<CacheFrequency> getRepository,
            IGenericListRepository<CacheFrequency> listRepository
        )
        {
            _context = context;
            _createRepository = createRepository;
            _updateRepository = updateRepository;
            _getRepository = getRepository;
            _listRepository = listRepository;
        }

        public async Task<IDbContextTransaction> BeginTransaction()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task<Result<CacheFrequency>> Create(CacheFrequency entity)
        {
            return await _createRepository.Create(_context, entity);
        }

        public async Task<Result<CacheFrequency>> Get(int id, bool NoTracking = true)
        {
            return await _getRepository.Get(_context, id, NoTracking);
        }

        public async Task<Result<CacheFrequency>> GetByEntity(string entity)
        {
            IQueryable<CacheFrequency> query = _context.CacheFrequencies.AsNoTracking();

            var frequency = await query.FirstOrDefaultAsync(f => f.Entity == entity && !f.Deleted);

            if (frequency == null) 
            {
                return Result<CacheFrequency>.Failure(new List<Error> { new Error("CacheFrequency.NotFound", "A frequência desta entidade não foi encontrada!") });
            }

            return Result<CacheFrequency>.Success(frequency);
        }

        public async Task<Result<List<CacheFrequency>>> List()
        {
            return await _listRepository.List(_context);
        }

        public async Task<Result<CacheFrequency>> Update(CacheFrequency entity)
        {
            return await _updateRepository.Update(_context, entity);
        }
    }
}

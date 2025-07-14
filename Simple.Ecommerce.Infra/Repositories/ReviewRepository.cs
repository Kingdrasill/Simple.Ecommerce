using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.ReviewEntity;
using Simple.Ecommerce.Infra.Interfaces.Generic;

namespace Simple.Ecommerce.Infra.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly TesteDbContext _context;
        private readonly IGenericCreateRepository<Review> _createRepository;
        private readonly IGenericDeleteRepository<Review> _deleteRepository;
        private readonly IGenericGetRepository<Review> _getRepository;
        private readonly IGenericListRepository<Review> _listRepository;
        private readonly IGenericUpdateRepository<Review> _updateRepository;

        public ReviewRepository(
            TesteDbContext context, 
            IGenericCreateRepository<Review> createRepository, 
            IGenericDeleteRepository<Review> deleteRepository, 
            IGenericGetRepository<Review> getRepository, 
            IGenericListRepository<Review> listRepository, 
            IGenericUpdateRepository<Review> updateRepository
        )
        {
            _context = context;
            _createRepository = createRepository;
            _deleteRepository = deleteRepository;
            _getRepository = getRepository;
            _listRepository = listRepository;
            _updateRepository = updateRepository;
        }

        public async Task<Result<Review>> Create(Review entity, bool skipSave = false)
        {
            return await _createRepository.Create(_context, entity);
        }

        public async Task<Result<bool>> Delete(int id, bool skipSave = false)
        {
            return await _deleteRepository.Delete(_context, id);
        }

        public async Task<Result<Review>> Get(int id, bool NoTracking = true)
        {
            return await _getRepository.Get(_context, id, NoTracking);
        }

        public async Task<Result<List<Review>>> List()
        {
            return await _listRepository.List(_context);
        }

        public async Task<Result<Review>> Update(Review entity, bool skipSave = false)
        {
            return await _updateRepository.Update(_context, entity);
        }
    }
}

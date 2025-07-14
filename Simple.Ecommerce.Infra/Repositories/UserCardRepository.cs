using Microsoft.EntityFrameworkCore;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.UserCardEntity;
using Simple.Ecommerce.Infra.Interfaces.Generic;

namespace Simple.Ecommerce.Infra.Repositories
{
    public class UserCardRepository : IUserCardRepository
    {
        private readonly TesteDbContext _context;
        private readonly IGenericCreateRepository<UserCard> _createRepository;
        private readonly IGenericDeleteRepository<UserCard> _deleteRepository;
        private readonly IGenericGetRepository<UserCard> _getRepository;
        private readonly IGenericListRepository<UserCard> _listRepository;

        public UserCardRepository(
            TesteDbContext context, 
            IGenericCreateRepository<UserCard> createRepository, 
            IGenericDeleteRepository<UserCard> deleteRepository, 
            IGenericGetRepository<UserCard> getRepository, 
            IGenericListRepository<UserCard> listRepository
        )
        {
            _context = context;
            _createRepository = createRepository;
            _deleteRepository = deleteRepository;
            _getRepository = getRepository;
            _listRepository = listRepository;
        }

        public async Task<Result<UserCard>> Create(UserCard entity, bool skipSave = false)
        {
            return await _createRepository.Create(_context, entity);
        }

        public async Task<Result<bool>> Delete(int id, bool skipSave = false)
        {
            return await _deleteRepository.Delete(_context, id);
        }

        public Task<Result<UserCard>> Get(int id, bool NoTracking = true)
        {
            return _getRepository.Get(_context, id, NoTracking);
        }

        public async Task<Result<List<UserCard>>> GetByUserId(int userId)
        {
            var list = await _context.UserCards
                .Where(uc => uc.UserId == userId && !uc.Deleted)
                .ToListAsync();

            return Result<List<UserCard>>.Success(list);
        }

        public Task<Result<List<UserCard>>> List()
        {
            return _listRepository.List(_context);
        }
    }
}

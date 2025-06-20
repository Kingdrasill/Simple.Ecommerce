using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.Domain.Entities.UserAddressEntity;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.Infra.Interfaces.Generic;
using Microsoft.EntityFrameworkCore;

namespace Simple.Ecommerce.Infra.Repositories
{
    public class UserAddressRepository : IUserAddressRepository
    {
        private readonly TesteDbContext _context;
        private readonly IGenericCreateRepository<UserAddress> _createRepository;
        private readonly IGenericDeleteRepository<UserAddress> _deleteRepository;
        private readonly IGenericGetRepository<UserAddress> _getRepository;
        private readonly IGenericListRepository<UserAddress> _listRepository;

        public UserAddressRepository(
            TesteDbContext context, 
            IGenericCreateRepository<UserAddress> createRepository, 
            IGenericDeleteRepository<UserAddress> deleteRepository, 
            IGenericGetRepository<UserAddress> getRepository, 
            IGenericListRepository<UserAddress> listRepository
        )
        {
            _context = context;
            _createRepository = createRepository;
            _deleteRepository = deleteRepository;
            _getRepository = getRepository;
            _listRepository = listRepository;
        }

        public async Task<Result<UserAddress>> Create(UserAddress entity)
        {
            return await _createRepository.Create(_context, entity);
        }

        public async Task<Result<bool>> Delete(int id)
        {
            return await _deleteRepository.Delete(_context, id);
        }

        public async Task<Result<UserAddress>> Get(int id, bool NoTracking = true)
        {
            return await _getRepository.Get(_context, id, NoTracking);
        }

        public async Task<Result<List<UserAddress>>> List()
        {
            return await _listRepository.List(_context);
        }

        public async Task<Result<List<UserAddress>>> GetByUser(int userId)
        {
            var list = await _context.UserAddresses
                                        .Where(ua => ua.UserId == userId && !ua.Deleted)
                                        .ToListAsync();

            return Result<List<UserAddress>>.Success(list);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.UserPaymentEntity;
using Simple.Ecommerce.Infra.Interfaces.Generic;

namespace Simple.Ecommerce.Infra.Repositories
{
    public class UserPaymentRepository : IUserPaymentRepository
    {
        private readonly TesteDbContext _context;
        private readonly IGenericCreateRepository<UserPayment> _createRepository;
        private readonly IGenericDeleteRepository<UserPayment> _deleteRepository;
        private readonly IGenericGetRepository<UserPayment> _getRepository;
        private readonly IGenericListRepository<UserPayment> _listRepository;

        public UserPaymentRepository(
            TesteDbContext context, 
            IGenericCreateRepository<UserPayment> createRepository, 
            IGenericDeleteRepository<UserPayment> deleteRepository, 
            IGenericGetRepository<UserPayment> getRepository, 
            IGenericListRepository<UserPayment> listRepository
        )
        {
            _context = context;
            _createRepository = createRepository;
            _deleteRepository = deleteRepository;
            _getRepository = getRepository;
            _listRepository = listRepository;
        }

        public async Task<Result<UserPayment>> Create(UserPayment entity, bool skipSave = false)
        {
            return await _createRepository.Create(_context, entity);
        }

        public async Task<Result<bool>> Delete(int id, bool skipSave = false)
        {
            return await _deleteRepository.Delete(_context, id);
        }

        public Task<Result<UserPayment>> Get(int id, bool NoTracking = true)
        {
            return _getRepository.Get(_context, id, NoTracking);
        }

        public async Task<Result<List<UserPayment>>> GetByUserId(int userId)
        {
            var list = await _context.UserPayments
                .Where(uc => uc.UserId == userId && !uc.Deleted)
                .ToListAsync();

            return Result<List<UserPayment>>.Success(list);
        }

        public Task<Result<List<UserPayment>>> List()
        {
            return _listRepository.List(_context);
        }
    }
}

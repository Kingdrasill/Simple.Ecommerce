using Microsoft.EntityFrameworkCore;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Infra.Interfaces.Generic;

namespace Simple.Ecommerce.Infra.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly TesteDbContext _context;
        private readonly IGenericCreateRepository<User> _createRepository;
        private readonly IGenericDeleteRepository<User> _deleteRepository;
        private readonly IGenericGetRepository<User> _getRepository;
        private readonly IGenericListRepository<User> _listRepository;
        private readonly IGenericUpdateRepository<User> _updateRepository;

        public UserRepository(
            TesteDbContext context,
            IGenericCreateRepository<User> createRepository,
            IGenericDeleteRepository<User> deleteRepository,
            IGenericGetRepository<User> getRepository,
            IGenericListRepository<User> listRepository,
            IGenericUpdateRepository<User> updateRepository)
        {
            _context = context;
            _createRepository = createRepository;
            _deleteRepository = deleteRepository;
            _getRepository = getRepository;
            _listRepository = listRepository;
            _updateRepository = updateRepository;
        }

        public async Task<Result<User>> Create(User entity, bool skipSave = false)
        {
            return await _createRepository.Create(_context, entity, skipSave);
        }

        public async Task<Result<bool>> Delete(int id, bool skipSave = false)
        {
            return await _deleteRepository.Delete(_context, id);
        }

        public async Task<Result<bool>> DeletePhotoFromUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(e => e.Id == id && !e.Deleted);
            if (user is null)
            {
                return Result<bool>.Failure(new List<Error> { new Error("NotFound", "Usuário não foi encontrado!") });
            }

            user.AddOrUpdatePhoto(null);
            if (user.Validate() is { IsFailure: true } result)
            {
                return Result<bool>.Failure(result.Errors!);
            }

            _context.Entry(user).Reference(u => u.Photo).IsModified = true;
            await _context.SaveChangesAsync();
            return Result<bool>.Success(true);
        }

        public async Task<Result<User>> Get(int id, bool NoTracking = true)
        {
            return await _getRepository.Get(_context, id, NoTracking);
        }

        public async Task<Result<User>> GetByEmail(string email)
        {
            IQueryable<User> query = _context.Users.AsNoTracking();

            var entity = await query.FirstOrDefaultAsync(u => u.Email == email && !u.Deleted);

            if (entity is null)
                return Result<User>.Failure(new List<Error> { new Error("NotFound", "Usuário não foi encontrado!") });

            return Result<User>.Success(entity);
        }

        public async Task<Result<List<User>>> GetByImageNames(List<string> imageNames)
        {
            var list = await _context.Users
                .Where(u => u.Photo != null && imageNames.Contains(u.Photo.FileName) && !u.Deleted)
                .ToListAsync();

            return Result<List<User>>.Success(list);
        }

        public async Task<Result<User>> GetByPhoneNumber(string phoneNumber)
        {
            IQueryable<User> query = _context.Users.AsNoTracking();

            var entity = await query.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber && !u.Deleted);

            if (entity is null)
                return Result<User>.Failure(new List<Error> { new Error("NotFound", "Usuário não foi encontrado!") });

            return Result<User>.Success(entity);
        }

        public async Task<Result<List<User>>> List()
        {
            return await _listRepository.List(_context);
        }

        public async Task<Result<User>> Update(User entity, bool skipSave = false)
        {
            return await _updateRepository.Update(_context, entity, skipSave);
        }
    }
}

using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cryptography;
using Simple.Ecommerce.Domain.Entities.LoginEntity;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.Infra.Interfaces.Generic;
using Microsoft.EntityFrameworkCore;

namespace Simple.Ecommerce.Infra.Repositories
{
    public class LoginRepository : ILoginRepository
    {
        private readonly TesteDbContext _context;
        private readonly IGenericCreateRepository<Login> _createRepository;
        private readonly IGenericDeleteRepository<Login> _deleteRepository;
        private readonly IGenericUpdateRepository<Login> _updateRepository;
        private readonly IGenericGetRepository<Login> _getRepository;
        private readonly IGenericListRepository<Login> _listRepository;
        private readonly ICryptographyService _cryptographyService;

        public LoginRepository(
            TesteDbContext context, 
            IGenericCreateRepository<Login> createRepository, 
            IGenericDeleteRepository<Login> deleteRepository,
            IGenericUpdateRepository<Login> updateRepository,
            IGenericGetRepository<Login> getRepository,
            IGenericListRepository<Login> listRepository,
            ICryptographyService cryptographyService
        )
        {
            _context = context;
            _createRepository = createRepository;
            _deleteRepository = deleteRepository;
            _updateRepository = updateRepository;
            _getRepository = getRepository;
            _listRepository = listRepository;
            _cryptographyService = cryptographyService;
        }

        public async Task<Result<Login>> Authenticate(string credential, string password)
        {
            var login = await _context.Logins.FirstOrDefaultAsync(l => l.Credential == credential && !l.Deleted);

            if (login == null)
            {
                return Result<Login>.Failure(new List<Error>{ new Error("Forbidden", "Credencial ou senha incorretos!") });
            }

            if (_cryptographyService.VerifyPassword(password, login.Password).IsFailure)
            {
                return Result<Login>.Failure(new List<Error>{ new Error("Forbidden", "Credencial ou senha incorretos!") });
            }

            return Result<Login>.Success(login);
        }

        public async Task<Result<Login>> Create(Login entity)
        {
            return await _createRepository.Create(_context, entity);
        }

        public async Task<Result<bool>> Delete(int id)
        {
            return await _deleteRepository.Delete(_context, id);
        }

        public async Task<Result<bool>> Find(int id)
        {
            var entity = await _context.Logins.FirstOrDefaultAsync(e => e.Id == id && !e.Deleted);

            if (entity is null)
                return Result<bool>.Failure(new List<Error> { new Error("NotFound", "Login não foi encontrado!") });

            return Result<bool>.Success(true);
        }

        public async Task<Result<Login>> Get(int id, bool NoTracking = true)
        {
            return await _getRepository.Get(_context, id, NoTracking);
        }

        public async Task<Result<Login>> GetByCredential(string credential)
        {
            var entity = await _context.Logins.FirstOrDefaultAsync(e => e.Credential == credential && !e.Deleted);

            if (entity is null)
                return Result<Login>.Failure(new List<Error> { new Error("NotFound", "Login não foi encontrado!") });

            return Result<Login>.Success(entity);
        }

        public async Task<Result<List<Login>>> List()
        {
            return await _listRepository.List(_context);    
        }

        public async Task<Result<Login>> Update(Login entity)
        {
            return await _updateRepository.Update(_context, entity);
        }
    }
}

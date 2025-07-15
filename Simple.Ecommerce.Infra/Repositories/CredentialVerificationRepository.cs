using Microsoft.EntityFrameworkCore;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.CredentialVerificationEntity;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Infra.Interfaces.Generic;

namespace Simple.Ecommerce.Infra.Repositories
{
    public class CredentialVerificationRepository : ICredentialVerificationRepository
    {
        private readonly TesteDbContext _context;
        private readonly IGenericCreateRepository<CredentialVerification> _createRepository;
        private readonly IGenericUpdateRepository<CredentialVerification> _updateRepository;

        public CredentialVerificationRepository(
            TesteDbContext context, 
            IGenericCreateRepository<CredentialVerification> createRepository, 
            IGenericUpdateRepository<CredentialVerification> updateRepository
        )
        {
            _context = context;
            _createRepository = createRepository;
            _updateRepository = updateRepository;
        }

        public async Task<Result<CredentialVerification>> Create(CredentialVerification entity, bool skipSave = false)
        {
            return await _createRepository.Create(_context, entity, skipSave);
        }

        public async Task<Result<CredentialVerification>> GetByToken(string token)
        {
            var credentialVerification = await _context.CredentialVerifications.FirstOrDefaultAsync(vc => vc.Token == token && !vc.Deleted);

            if (credentialVerification == null)
            {
                var erros = new List<Error>();
                erros.Add(new Error("CredentialVerification.GetByToken.NotFound", "A verificação da credential do token passado não foi encontrada!"));

                return Result<CredentialVerification>.Failure(erros);
            }

            return Result<CredentialVerification>.Success(credentialVerification);
        }

        public async Task<Result<CredentialVerification>> Update(CredentialVerification entity, bool skipSave = false)
        {
            return await _updateRepository.Update(_context, entity, skipSave);
        }
    }
}

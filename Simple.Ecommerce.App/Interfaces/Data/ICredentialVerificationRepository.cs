using Simple.Ecommerce.App.Interfaces.Data.BaseRepository;
using Simple.Ecommerce.Domain.Entities.CredentialVerificationEntity;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Data
{
    public interface ICredentialVerificationRepository :
        IBaseCreateRepository<CredentialVerification>,
        IBaseUpdateRepository<CredentialVerification>
    {
        Task<Result<CredentialVerification>> GetByToken(string token);
    }
}

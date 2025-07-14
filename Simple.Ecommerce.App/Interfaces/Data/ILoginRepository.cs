using Simple.Ecommerce.App.Interfaces.Data.BaseRepository;
using Simple.Ecommerce.Domain.Entities.LoginEntity;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Data
{
    public interface ILoginRepository :
        IBaseCreateRepository<Login>,
        IBaseDeleteRepository<Login>,
        IBaseGetRepository<Login>,
        IBaseListRepository<Login>,
        IBaseUpdateRepository<Login>
    {
        Task<Result<Login>> Authenticate(string credential, string password);
        Task<Result<Login>> GetByCredential(string credential);
        Task<Result<bool>> Find(int id);
    }
}

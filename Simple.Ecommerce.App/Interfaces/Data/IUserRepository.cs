using Simple.Ecommerce.App.Interfaces.Data.BaseRepository;
using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Data
{
    public interface IUserRepository :
        IBaseCreateRepository<User>,
        IBaseDeleteRepository<User>,
        IBaseUpdateRepository<User>,
        IBaseGetRepository<User>,
        IBaseListRepository<User>
    {
        Task<Result<User>> GetByEmail(string email);
        Task<Result<List<User>>> GetByImageNames(List<string> imageNames);
        Task<Result<User>> GetByPhoneNumber(string phoneNumber);
        Task<Result<bool>> DeletePhotoFromUser(int id);
    }
}

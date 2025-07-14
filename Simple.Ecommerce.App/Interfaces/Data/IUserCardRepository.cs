using Simple.Ecommerce.App.Interfaces.Data.BaseRepository;
using Simple.Ecommerce.Domain.Entities.UserCardEntity;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Data
{
    public interface IUserCardRepository :
        IBaseCreateRepository<UserCard>,
        IBaseDeleteRepository<UserCard>,
        IBaseGetRepository<UserCard>,
        IBaseListRepository<UserCard>
    {
        Task<Result<List<UserCard>>> GetByUserId(int userId);
    }
}

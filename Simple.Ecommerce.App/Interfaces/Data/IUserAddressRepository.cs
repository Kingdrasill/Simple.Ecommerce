using Simple.Ecommerce.App.Interfaces.Data.BaseRepository;
using Simple.Ecommerce.Domain.Entities.UserAddressEntity;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Data
{
    public interface IUserAddressRepository :
        IBaseCreateRepository<UserAddress>,
        IBaseDeleteRepository<UserAddress>,
        IBaseGetRepository<UserAddress>,
        IBaseListRepository<UserAddress>
    {
        Task<Result<List<UserAddress>>> GetByUser(int userId);
    }
}

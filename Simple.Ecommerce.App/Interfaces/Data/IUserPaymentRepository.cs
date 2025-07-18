using Simple.Ecommerce.App.Interfaces.Data.BaseRepository;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.UserPaymentEntity;

namespace Simple.Ecommerce.App.Interfaces.Data
{
    public interface IUserPaymentRepository :
        IBaseCreateRepository<UserPayment>,
        IBaseDeleteRepository<UserPayment>,
        IBaseGetRepository<UserPayment>,
        IBaseListRepository<UserPayment>
    {
        Task<Result<List<UserPayment>>> GetByUserId(int userId);
    }
}

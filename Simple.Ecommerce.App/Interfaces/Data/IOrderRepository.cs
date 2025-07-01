using Simple.Ecommerce.App.Interfaces.Data.BaseRepository;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Data
{
    public interface IOrderRepository :
        IBaseCreateRepository<Order>,
        IBaseDeleteRepository<Order>,
        IBaseGetRepository<Order>,
        IBaseListRepository<Order>,
        IBaseUpdateRepository<Order>
    {
        Task<Result<bool>> DeletePaymentMethod(int id);
    }
}

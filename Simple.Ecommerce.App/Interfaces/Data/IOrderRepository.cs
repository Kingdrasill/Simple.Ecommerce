using Simple.Ecommerce.App.Interfaces.Data.BaseRepository;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Contracts.OrderContracts;

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
        Task<Result<bool>> GetFirstPurchase(int id);
        Task<Result<OrderDiscountDTO?>> GetDiscountDTOById(int id);
        Task<Result<OrderCompleteDTO>> GetCompleteOrder(int id);
    }
}

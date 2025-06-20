using Simple.Ecommerce.App.Interfaces.Data.BaseRepository;
using Simple.Ecommerce.Domain.Entities.OrderDiscountEnity;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Data
{
    public interface IOrderDiscountRepository :
        IBaseCreateRepository<OrderDiscount>,
        IBaseDeleteRepository<OrderDiscount>,
        IBaseGetRepository<OrderDiscount>,
        IBaseListRepository<OrderDiscount>
    {
        Task<Result<List<OrderDiscount>>> GetByOrderId(int ordereId);
        Task<Result<List<OrderDiscount>>> GetByDiscountId(int discountId);
    }
}

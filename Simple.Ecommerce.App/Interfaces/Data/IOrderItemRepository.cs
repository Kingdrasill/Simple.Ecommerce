using Simple.Ecommerce.App.Interfaces.Data.BaseRepository;
using Simple.Ecommerce.Contracts.OrderItemContracts;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Data
{
    public interface IOrderItemRepository :
        IBaseCreateRepository<OrderItem>,
        IBaseDeleteRepository<OrderItem>,
        IBaseGetRepository<OrderItem>,
        IBaseListRepository<OrderItem>,
        IBaseUpdateRepository<OrderItem>
    {
        Task<Result<List<OrderItem>>> GetByOrderId(int orderId);
        Task<Result<OrderItem>> GetByOrderIdAndProductId(int orderId, int productId);
        Task<Result<List<OrderItemDiscountInfoDTO>>> GetOrdemItemsDiscountInfo(int orderId);
    }
}

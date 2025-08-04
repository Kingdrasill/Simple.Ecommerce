using Simple.Ecommerce.App.Interfaces.Data.BaseRepository;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Contracts.OrderItemContracts.Discounts;

namespace Simple.Ecommerce.App.Interfaces.Data
{
    public interface IOrderItemRepository :
        IBaseCreateRepository<OrderItem>,
        IBaseDeleteRepository<OrderItem>,
        IBaseGetRepository<OrderItem>,
        IBaseListRepository<OrderItem>,
        IBaseUpdateRepository<OrderItem>,
        IBaseDetachRepository<OrderItem>
    {
        Task<Result<List<OrderItem>>> GetByOrderId(int orderId);
        Task<Result<OrderItem>> GetByOrderIdAndProductId(int orderId, int productId);
        Task<Result<List<OrderItemInfoDTO>>> ListByOrderIdOrderItemInfoDTO(int orderId);
        Task<Result<List<OrderItemWithDiscountDTO>>> ListByOrderIdOrderItemWithDiscountDTO(int orderId);
    }
}

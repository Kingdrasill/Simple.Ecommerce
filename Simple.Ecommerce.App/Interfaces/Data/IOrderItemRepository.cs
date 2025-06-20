using Simple.Ecommerce.App.Interfaces.Data.BaseRepository;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;

namespace Simple.Ecommerce.App.Interfaces.Data
{
    public interface IOrderItemRepository :
        IBaseCreateRepository<OrderItem>,
        IBaseDeleteRepository<OrderItem>,
        IBaseGetRepository<OrderItem>,
        IBaseListRepository<OrderItem>
    {
    }
}

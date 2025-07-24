using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.ReadData;

namespace Simple.Ecommerce.App.Interfaces.Services.UnitOfWork
{
    public interface IRevertedOrderUnitOfWork : IBaseUnitOfWork
    {
        IUserRepository Users { get; }
        IOrderRepository Orders { get; }
        IOrderItemRepository OrderItems { get; }
        IDiscountRepository Discounts { get; }
        IProductRepository Products { get; }
        IOrderDetailReadModelRepository OrderDetails { get; }
        IOrderEventStreamReadModelRepository OrderEventStreams { get; }
    }
}

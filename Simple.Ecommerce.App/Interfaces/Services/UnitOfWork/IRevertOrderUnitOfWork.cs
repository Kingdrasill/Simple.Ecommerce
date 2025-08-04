using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.ReadData;

namespace Simple.Ecommerce.App.Interfaces.Services.UnitOfWork
{
    public interface IRevertOrderUnitOfWork : IBaseUnitOfWork
    {
        IUserRepository Users { get; }
        IProductRepository Products { get; }
        IOrderRepository Orders { get; }
        IOrderItemRepository OrderItems { get; }
        IDiscountRepository Discounts { get; }
        ICouponRepository Coupons { get; }
        IOrderDetailReadModelRepository OrderDetails { get; }
        IOrderEventStreamReadModelRepository OrderEventStreams { get; }
    }
}

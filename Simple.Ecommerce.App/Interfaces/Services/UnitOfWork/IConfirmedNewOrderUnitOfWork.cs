using Simple.Ecommerce.App.Interfaces.Data;

namespace Simple.Ecommerce.App.Interfaces.Services.UnitOfWork
{
    public interface IConfirmedNewOrderUnitOfWork : IBaseUnitOfWork
    {
        IOrderRepository Orders { get; }
        IUserRepository Users { get; }
        IDiscountRepository Discounts { get; }
        IOrderItemRepository OrderItems { get; }
        IProductRepository Products { get; }
        IProductDiscountRepository ProductDiscounts { get; }
        IDiscountBundleItemRepository DiscountBundleItems { get; }
    }
}

using Simple.Ecommerce.App.Interfaces.Data;

namespace Simple.Ecommerce.App.Interfaces.Services.UnitOfWork
{
    public interface IConfirmOrderUnitOfWork : IBaseUnitOfWork
    {
        IUserRepository Users { get; }
        IOrderRepository Orders { get; }
        IOrderItemRepository OrderItems { get; }
        IProductRepository Products { get; }
        IProductDiscountRepository ProductDiscounts { get; }
        IDiscountRepository Discounts { get; }
        IDiscountTierRepository DiscountTiers { get; }
        IDiscountBundleItemRepository DiscountBundleItems { get; }
        ICouponRepository Coupons { get; }
    }
}

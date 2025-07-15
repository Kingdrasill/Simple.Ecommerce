using Simple.Ecommerce.App.Interfaces.Data;

namespace Simple.Ecommerce.App.Interfaces.Services.UnitOfWork
{
    public interface ICreateBatchCouponUnitOfWork : IBaseUnitOfWork
    {
        IDiscountRepository Discounts { get; }
        ICouponRepository Coupons { get; }
    }
}

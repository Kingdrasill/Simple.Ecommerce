using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;

namespace Simple.Ecommerce.Infra.Services.UnitOfWork
{
    public class CreateBatchCouponUnitOfWork : BaseUnitOfWork, ICreateBatchCouponUnitOfWork
    {
        public IDiscountRepository Discounts { get; }
        public ICouponRepository Coupons { get; }

        public CreateBatchCouponUnitOfWork(
            TesteDbContext context, 
            IDiscountRepository discounts, 
            ICouponRepository coupons
        ) : base(context)
        {
            Discounts = discounts;
            Coupons = coupons;
        }
    }
}

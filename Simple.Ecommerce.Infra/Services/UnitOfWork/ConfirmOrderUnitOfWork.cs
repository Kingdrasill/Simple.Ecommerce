using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;

namespace Simple.Ecommerce.Infra.Services.UnitOfWork
{
    public class ConfirmOrderUnitOfWork : BaseUnitOfWork, IConfirmOrderUnitOfWork
    {
        public IUserRepository Users { get; }
        public IOrderRepository Orders { get; }
        public IOrderItemRepository OrderItems { get; }
        public IProductRepository Products { get; }
        public IProductDiscountRepository ProductDiscounts { get; }
        public IDiscountRepository Discounts { get; }
        public IDiscountTierRepository DiscountTiers { get; }
        public IDiscountBundleItemRepository DiscountBundleItems { get; }
        public ICouponRepository Coupons { get; }

        public ConfirmOrderUnitOfWork(
            TesteDbContext context, 
            IUserRepository users, 
            IOrderRepository orders, 
            IOrderItemRepository orderItems, 
            IProductRepository products, 
            IProductDiscountRepository productDiscounts, 
            IDiscountRepository discounts, 
            IDiscountTierRepository discountTiers, 
            IDiscountBundleItemRepository discountBundleItems, 
            ICouponRepository coupons
        ) : base(context)
        {
            Users = users;
            Orders = orders;
            OrderItems = orderItems;
            Products = products;
            ProductDiscounts = productDiscounts;
            Discounts = discounts;
            DiscountTiers = discountTiers;
            DiscountBundleItems = discountBundleItems;
            Coupons = coupons;
        }
    }
}

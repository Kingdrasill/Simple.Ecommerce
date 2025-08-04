using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;

namespace Simple.Ecommerce.Infra.Services.UnitOfWork
{
    public class AddItemsOrderUnitOfWork : BaseUnitOfWork, IAddItemsOrderUnitOfWork
    {
        public IOrderItemRepository OrderItems { get; }
        public IOrderRepository Orders { get; }
        public IProductRepository Products { get; }
        public IProductDiscountRepository ProductDiscounts { get; }
        public IDiscountRepository Discounts { get; }
        public IDiscountBundleItemRepository DiscountBundleItems { get; }
        public ICouponRepository Coupons { get; }

        public AddItemsOrderUnitOfWork(
            TesteDbContext context, 
            IOrderItemRepository orderItems, 
            IOrderRepository orders, 
            IProductRepository products, 
            IProductDiscountRepository productDiscounts,
            IDiscountRepository discounts, 
            IDiscountBundleItemRepository discountBundleItems,
            ICouponRepository coupons
        ) : base(context)
        {
            OrderItems = orderItems;
            Orders = orders;
            Products = products;
            ProductDiscounts = productDiscounts;
            Discounts = discounts;
            DiscountBundleItems = discountBundleItems;
            Coupons = coupons;
        }
    }
}

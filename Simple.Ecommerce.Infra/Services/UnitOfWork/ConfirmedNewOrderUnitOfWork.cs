using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;

namespace Simple.Ecommerce.Infra.Services.UnitOfWork
{
    public class ConfirmedNewOrderUnitOfWork : BaseUnitOfWork, IConfirmedNewOrderUnitOfWork
    {
        public IOrderRepository Orders { get; }
        public IUserRepository Users { get; }
        public IDiscountRepository Discounts { get; }
        public IOrderItemRepository OrderItems { get; }
        public IProductRepository Products { get; }
        public IProductDiscountRepository ProductDiscounts { get; }
        public IDiscountBundleItemRepository DiscountBundleItems { get; }
        
        public ConfirmedNewOrderUnitOfWork(
            TesteDbContext context,
            IOrderRepository orders,
            IUserRepository users,
            IDiscountRepository discounts,
            IOrderItemRepository orderItems,
            IProductRepository products,
            IProductDiscountRepository productDiscounts,
            IDiscountBundleItemRepository discountBundleItems
        ) : base(context)
        {
            Orders = orders;
            Users = users;
            Discounts = discounts;
            OrderItems = orderItems;
            Products = products;
            ProductDiscounts = productDiscounts;
            DiscountBundleItems = discountBundleItems;
        }
    }
}

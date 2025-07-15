using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;

namespace Simple.Ecommerce.Infra.Services.UnitOfWork
{
    public class AddItemsOrderUnitOfWork : BaseUnitOfWork, IAddItemsOrderUnitOfWork
    {
        public IOrderItemRepository OrderItems { get; }
        public IOrderRepository Orders { get; }
        public IProductRepository Products { get; }
        public IDiscountRepository Discounts { get; }
        public IDiscountBundleItemRepository DiscountBundleItems { get; }

        public AddItemsOrderUnitOfWork(
            TesteDbContext context, 
            IOrderItemRepository orderItems, 
            IOrderRepository orders, 
            IProductRepository products, 
            IDiscountRepository discounts, 
            IDiscountBundleItemRepository discountBundleItems
        ) : base(context)
        {
            OrderItems = orderItems;
            Orders = orders;
            Products = products;
            Discounts = discounts;
            DiscountBundleItems = discountBundleItems;
        }
    }
}

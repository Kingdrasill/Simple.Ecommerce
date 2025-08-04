using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.ReadData;
using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;

namespace Simple.Ecommerce.Infra.Services.UnitOfWork
{
    public class RevertOrderUnitOfWork : BaseUnitOfWork, IRevertOrderUnitOfWork
    {
        public IUserRepository Users { get; }
        public IProductRepository Products { get; }
        public IOrderRepository Orders { get; }
        public IOrderItemRepository OrderItems { get; }
        public IDiscountRepository Discounts { get; }
        public ICouponRepository Coupons { get; }
        public IOrderDetailReadModelRepository OrderDetails { get; }
        public IOrderEventStreamReadModelRepository OrderEventStreams { get; }
        
        public RevertOrderUnitOfWork(
            TesteDbContext context,
            IUserRepository users,
            IProductRepository products,
            IOrderRepository orders,
            IOrderItemRepository orderItems,
            IDiscountRepository discounts,
            ICouponRepository coupons,
            IOrderDetailReadModelRepository orderDetails,
            IOrderEventStreamReadModelRepository orderEventStreams
        ) : base(context) 
        { 
            Users = users;
            Products = products;
            Orders = orders;
            OrderItems = orderItems;
            Discounts = discounts;
            Coupons = coupons;
            OrderDetails = orderDetails;
            OrderEventStreams = orderEventStreams;
        }
    }
}

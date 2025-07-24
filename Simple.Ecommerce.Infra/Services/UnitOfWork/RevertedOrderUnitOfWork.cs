using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.ReadData;
using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;

namespace Simple.Ecommerce.Infra.Services.UnitOfWork
{
    public class RevertedOrderUnitOfWork : BaseUnitOfWork, IRevertedOrderUnitOfWork
    {
        public IUserRepository Users { get; }
        public IOrderRepository Orders { get; }
        public IOrderItemRepository OrderItems { get; }
        public IDiscountRepository Discounts { get; }
        public IProductRepository Products { get; }
        public IOrderDetailReadModelRepository OrderDetails { get; }
        public IOrderEventStreamReadModelRepository OrderEventStreams { get; }
        
        public RevertedOrderUnitOfWork(
            TesteDbContext context,
            IUserRepository users,
            IOrderRepository orders,
            IOrderItemRepository orderItems,
            IDiscountRepository discounts,
            IProductRepository products,
            IOrderDetailReadModelRepository orderDetails,
            IOrderEventStreamReadModelRepository orderEventStreams
        ) : base(context) 
        { 
            Users = users;
            Orders = orders;
            OrderItems = orderItems;
            Discounts = discounts;
            Products = products;
            OrderDetails = orderDetails;
            OrderEventStreams = orderEventStreams;
        }
    }
}

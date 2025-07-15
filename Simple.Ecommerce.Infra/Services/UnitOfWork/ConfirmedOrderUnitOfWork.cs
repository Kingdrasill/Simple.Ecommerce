using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;

namespace Simple.Ecommerce.Infra.Services.UnitOfWork
{
    public class ConfirmedOrderUnitOfWork : BaseUnitOfWork, IConfirmedOrderUnitOfWork
    {
        public IUserRepository Users { get; }
        public IOrderRepository Orders { get; }
        public IOrderItemRepository OrderItems { get; }

        public ConfirmedOrderUnitOfWork(
            TesteDbContext context, 
            IUserRepository users, 
            IOrderRepository orders, 
            IOrderItemRepository orderItems
        ) : base(context)
        {
            Users = users;
            Orders = orders;
            OrderItems = orderItems;
        }
    }
}

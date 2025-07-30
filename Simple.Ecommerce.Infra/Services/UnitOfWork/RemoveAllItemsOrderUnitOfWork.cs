using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;

namespace Simple.Ecommerce.Infra.Services.UnitOfWork
{
    public class RemoveAllItemsOrderUnitOfWork : BaseUnitOfWork, IRemoveAllItemsOrderUnitOfWork
    {
        public IOrderRepository Orders { get; }
        public IOrderItemRepository OrderItems { get; }

        public RemoveAllItemsOrderUnitOfWork(
            TesteDbContext context, 
            IOrderRepository orders,
            IOrderItemRepository orderItems
        ) : base(context) 
        {
            Orders = orders;
            OrderItems = orderItems;
        }
    }
}

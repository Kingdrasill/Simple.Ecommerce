using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;

namespace Simple.Ecommerce.Infra.Services.UnitOfWork
{
    public class RemoveAllItemsOrderUnitOfWork : BaseUnitOfWork, IRemoveAllItemsOrderUnitOfWork
    {
        public IOrderItemRepository OrderItems { get; }

        public RemoveAllItemsOrderUnitOfWork(
            TesteDbContext context, 
            IOrderItemRepository orderItems
        ) : base(context) 
        {
            OrderItems = orderItems;
        }
    }
}

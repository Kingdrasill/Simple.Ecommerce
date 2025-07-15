using Simple.Ecommerce.App.Interfaces.Data;

namespace Simple.Ecommerce.App.Interfaces.Services.UnitOfWork
{
    public interface IRemoveAllItemsOrderUnitOfWork : IBaseUnitOfWork
    {
        IOrderItemRepository OrderItems { get; }
    }
}

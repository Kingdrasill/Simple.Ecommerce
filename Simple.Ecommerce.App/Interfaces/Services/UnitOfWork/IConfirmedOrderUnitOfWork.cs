using Simple.Ecommerce.App.Interfaces.Data;

namespace Simple.Ecommerce.App.Interfaces.Services.UnitOfWork
{
    public interface IConfirmedOrderUnitOfWork : IBaseUnitOfWork
    {
        IUserRepository Users { get; }
        IOrderRepository Orders { get; }
        IOrderItemRepository OrderItems { get; }
    }
}

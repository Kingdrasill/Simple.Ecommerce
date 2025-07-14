using Simple.Ecommerce.App.Interfaces.Data;

namespace Simple.Ecommerce.App.Interfaces.Services.UnityOfWork
{
    public interface IConfirmedOrderUnityOfWork : IBaseUnityOfWork
    {
        IUserRepository Users { get; }
        IOrderRepository Orders { get; }
        IOrderItemRepository OrderItems { get; }
    }
}

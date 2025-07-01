using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Commands.OrderCommands
{
    public interface IRemovePaymentMethodOrderCommand
    {
        Task<Result<bool>> Execute(int orderId);
    }
}

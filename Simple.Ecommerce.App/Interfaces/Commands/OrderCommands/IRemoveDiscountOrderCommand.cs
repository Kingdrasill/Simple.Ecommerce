using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Commands.OrderCommands
{
    public interface IRemoveDiscountOrderCommand
    {
        Task<Result<bool>> Execute(int orderId);
    }
}

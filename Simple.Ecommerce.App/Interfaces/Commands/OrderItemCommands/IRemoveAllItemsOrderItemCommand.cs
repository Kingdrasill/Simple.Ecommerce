using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Commands.OrderItemCommands
{
    public interface IRemoveAllItemsOrderItemCommand
    {
        Task<Result<bool>> Execute(int orderId);
    }
}

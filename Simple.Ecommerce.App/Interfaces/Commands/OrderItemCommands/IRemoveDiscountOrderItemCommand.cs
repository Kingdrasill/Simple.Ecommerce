using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Commands.OrderItemCommands
{
    public interface IRemoveDiscountOrderItemCommand
    {
        Task<Result<bool>> Execute(int orderId, int productId);
    }
}

using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Commands.OrderItemCommands
{
    public interface IRemoveDiscountOrderItemCommand
    {
        Task<Result<bool>> Execute(int orderId, int productId);
    }
}

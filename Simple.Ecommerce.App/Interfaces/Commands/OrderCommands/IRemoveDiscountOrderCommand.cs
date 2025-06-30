using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Commands.OrderCommands
{
    public interface IRemoveDiscountOrderCommand
    {
        Task<Result<bool>> Execute(int orderId);
    }
}

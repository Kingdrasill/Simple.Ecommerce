using Simple.Ecommerce.Contracts.OrderItemContracts;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Commands.OrderItemCommands
{
    public interface IAddItemOrderItemCommand
    {
        Task<Result<OrderItemResponse>> Execute(OrderItemRequest request);
    }
}

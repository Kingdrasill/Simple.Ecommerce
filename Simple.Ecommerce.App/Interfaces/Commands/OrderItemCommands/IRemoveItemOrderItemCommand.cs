using Simple.Ecommerce.Contracts.OrderItemContracts;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Commands.OrderItemCommands
{
    public interface IRemoveItemOrderItemCommand
    {
        Task<Result<OrderItemResponse?>> Execute(OrderItemRequest request);
    }
}

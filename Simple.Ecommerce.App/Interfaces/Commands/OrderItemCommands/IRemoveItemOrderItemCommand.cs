using Simple.Ecommerce.Contracts.OrderItemContracts;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Commands.OrderItemCommands
{
    public interface IRemoveItemOrderItemCommand
    {
        Task<Result<OrderItemResponse?>> Execute(OrderItemRequest request);
    }
}

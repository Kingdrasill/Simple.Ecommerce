using Simple.Ecommerce.Contracts.OrderItemContracts;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Commands.OrderItemCommands
{
    public interface IAddItemsOrderItemCommand
    {
        Task<Result<OrderItemsResponse>> Execute(OrderItemsRequest request);
    }
}

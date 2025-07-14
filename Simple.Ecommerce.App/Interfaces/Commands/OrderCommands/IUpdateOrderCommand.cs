using Simple.Ecommerce.Contracts.OrderContracts;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Commands.OrderCommands
{
    public interface IUpdateOrderCommand
    {
        Task<Result<OrderResponse>> Execute(OrderRequest request);
    }
}

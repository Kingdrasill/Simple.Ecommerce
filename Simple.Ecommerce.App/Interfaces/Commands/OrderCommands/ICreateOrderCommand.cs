using Simple.Ecommerce.Contracts.OrderContracts;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Commands.OrderCommands
{
    public interface ICreateOrderCommand
    {
        Task<Result<OrderResponse>> Execute(OrderRequest request);
    }
}

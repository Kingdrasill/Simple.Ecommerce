using Simple.Ecommerce.Contracts.OrderContracts;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Commands.OrderCommands
{
    public interface IUpdateOrderCommand
    {
        Task<Result<OrderResponse>> Execute(OrderRequest request);
    }
}

using Simple.Ecommerce.Contracts.OrderContracts;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Commands.OrderCommands
{
    public interface IRevertProcessedOrderCommand
    {
        Task<Result<OrderResponse>> Execute(int id);
    }
}

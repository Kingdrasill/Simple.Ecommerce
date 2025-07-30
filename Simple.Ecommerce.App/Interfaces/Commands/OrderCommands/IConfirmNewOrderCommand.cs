using Simple.Ecommerce.Contracts.OrderContracts.CompleteDTO;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Commands.OrderCommands
{
    public interface IConfirmNewOrderCommand
    {
        Task<Result<OrderCompleteDTO>> Execute(OrderCompleteRequest request);
    }
}

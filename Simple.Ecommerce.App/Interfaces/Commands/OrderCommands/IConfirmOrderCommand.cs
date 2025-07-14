using Simple.Ecommerce.Contracts.OrderContracts;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Commands.OrderCommands
{
    public interface IConfirmOrderCommand
    {
        Task<Result<OrderCompleteDTO>> Execute(int id);
    }
}

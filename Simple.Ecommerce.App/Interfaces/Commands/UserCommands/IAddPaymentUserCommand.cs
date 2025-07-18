using Simple.Ecommerce.Contracts.UserPaymentContracts;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Commands.UserCommands
{
    public interface IAddPaymentUserCommand
    {
        Task<Result<bool>> Execute(UserPaymentRequest request);
    }
}

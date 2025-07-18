using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Commands.UserCommands
{
    public interface IRemovePaymentUserCommand
    {
        Task<Result<bool>> Execute(int userCardId);
    }
}

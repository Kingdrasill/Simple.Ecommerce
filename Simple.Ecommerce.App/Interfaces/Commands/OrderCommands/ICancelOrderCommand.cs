using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Commands.OrderCommands
{
    public interface ICancelOrderCommand
    {
        Task<Result<bool>> Execute(int id);
    }
}

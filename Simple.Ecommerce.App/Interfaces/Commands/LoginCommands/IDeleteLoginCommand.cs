using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Commands.LoginCommands
{
    public interface IDeleteLoginCommand
    {
        Task<Result<bool>> Execute(int id);
    }
}

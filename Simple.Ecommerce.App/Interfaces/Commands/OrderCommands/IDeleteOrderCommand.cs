using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Commands.OrderCommands
{
    public interface IDeleteOrderCommand
    {
        Task<Result<bool>> Execute(int id);
    }
}

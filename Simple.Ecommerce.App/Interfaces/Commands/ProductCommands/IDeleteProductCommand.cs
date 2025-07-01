using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Commands.ProductCommands
{
    public interface IDeleteProductCommand
    {
        Task<Result<bool>> Execute(int id);
    }
}

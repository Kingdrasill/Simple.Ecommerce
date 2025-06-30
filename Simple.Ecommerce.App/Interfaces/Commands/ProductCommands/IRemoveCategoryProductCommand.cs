using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Commands.ProductCommands
{
    public interface IRemoveCategoryProductCommand
    {
        Task<Result<bool>> Execute(int id);
    }
}

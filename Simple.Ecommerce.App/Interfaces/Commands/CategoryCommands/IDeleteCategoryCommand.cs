using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Commands.CategoryCommands
{
    public interface IDeleteCategoryCommand
    {
        Task<Result<bool>> Execute(int id);
    }
}

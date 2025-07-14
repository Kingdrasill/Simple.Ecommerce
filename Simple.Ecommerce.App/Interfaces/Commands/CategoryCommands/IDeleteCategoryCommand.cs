using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Commands.CategoryCommands
{
    public interface IDeleteCategoryCommand
    {
        Task<Result<bool>> Execute(int id);
    }
}

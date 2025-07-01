using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Commands.CategoryCommands
{
    public interface IDeleteCategoryCommand
    {
        Task<Result<bool>> Execute(int id);
    }
}

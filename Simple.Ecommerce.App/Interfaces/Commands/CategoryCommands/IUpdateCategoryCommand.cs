using Simple.Ecommerce.Contracts.CategoryContracts;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Commands.CategoryCommands
{
    public interface IUpdateCategoryCommand
    {
        Task<Result<CategoryResponse>> Execute(CategoryRequest request);
    }
}

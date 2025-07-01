using Simple.Ecommerce.Contracts.CategoryContracts;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Commands.CategoryCommands
{
    public interface ICreateCategoryCommand
    {
        Task<Result<CategoryResponse>> Execute(CategoryRequest request);
    }
}

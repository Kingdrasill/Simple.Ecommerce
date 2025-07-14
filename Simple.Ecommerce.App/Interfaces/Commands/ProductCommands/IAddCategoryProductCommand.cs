using Simple.Ecommerce.Contracts.ProductCategoryContracts;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Commands.ProductCommands
{
    public interface IAddCategoryProductCommand
    {
        Task<Result<ProductCategoryDTO>> Execute(ProductCategoryRequest request);
    }
}

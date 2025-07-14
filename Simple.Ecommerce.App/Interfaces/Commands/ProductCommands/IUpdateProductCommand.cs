using Simple.Ecommerce.Contracts.ProductContracts;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Commands.ProductCommands
{
    public interface IUpdateProductCommand
    {
        Task<Result<ProductResponse>> Execute(ProductRequest request);
    }
}

using Simple.Ecommerce.Contracts.ProductContracts;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Commands.ProductCommands
{
    public interface IUpdateProductCommand
    {
        Task<Result<ProductResponse>> Execute(ProductRequest request);
    }
}

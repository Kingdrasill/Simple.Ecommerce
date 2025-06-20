using Simple.Ecommerce.Contracts.ProductContracts;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Commands.ProductCommands
{
    public interface ICreateProductCommand
    {
        Task<Result<ProductResponse>> Execute(ProductRequest request);
    }
}

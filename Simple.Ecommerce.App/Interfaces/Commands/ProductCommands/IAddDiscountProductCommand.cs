using Simple.Ecommerce.Contracts.ProductDiscountContracts;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Commands.ProductCommands
{
    public interface IAddDiscountProductCommand
    {
        Task<Result<ProductDiscountResponse>> Execute(ProductDiscountRequest request);
    }
}

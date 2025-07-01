using Simple.Ecommerce.Contracts.ProductDiscountContracts;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Commands.ProductCommands
{
    public interface IAddDiscountProductCommand
    {
        Task<Result<ProductDiscountResponse>> Execute(ProductDiscountRequest request);
    }
}

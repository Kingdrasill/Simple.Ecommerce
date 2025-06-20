using Simple.Ecommerce.Contracts.DiscountContracts;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands
{
    public interface IUpdateDiscountCommand
    {
        Task<Result<DiscountDTO>> Execute(DiscountRequest request);
    }
}

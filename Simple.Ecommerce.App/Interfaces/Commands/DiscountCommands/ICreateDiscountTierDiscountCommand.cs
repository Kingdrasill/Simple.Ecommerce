using Simple.Ecommerce.Contracts.DiscountTierContracts;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands
{
    public interface ICreateDiscountTierDiscountCommand
    {
        Task<Result<DiscountTierResponse>> Execute(DiscountTierRequest request);
    }
}

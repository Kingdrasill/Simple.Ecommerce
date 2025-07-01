using Simple.Ecommerce.Contracts.DiscountTierContracts;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands
{
    public interface IUpdateDiscountTierDiscountCommand
    {
        Task<Result<DiscountTierResponse>> Execute(DiscountTierRequest request);
    }
}

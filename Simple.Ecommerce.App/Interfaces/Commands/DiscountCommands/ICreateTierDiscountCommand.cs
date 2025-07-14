using Simple.Ecommerce.Contracts.DiscountTierContracts;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands
{
    public interface ICreateTierDiscountCommand
    {
        Task<Result<DiscountTierResponse>> Execute(DiscountTierRequest request);
    }
}

using Simple.Ecommerce.Contracts.DiscountContracts;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands
{
    public interface ICreateDiscountCommand
    {
        Task<Result<DiscountDTO>> Execute(DiscountRequest request);
    }
}

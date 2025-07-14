using Simple.Ecommerce.Contracts.DiscountBundleItemContracts;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands
{
    public interface ICreateBundleItemDiscountCommand
    {
        Task<Result<DiscountBundleItemResponse>> Execute(DiscountBundleItemRequest request);
    }
}
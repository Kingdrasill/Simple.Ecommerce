using Simple.Ecommerce.Contracts.DiscountBundleItemContracts;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands
{
    public interface IUpdateDiscountBundleItemDiscountCommand
    {
        Task<Result<DiscountBundleItemResponse>> Execute(DiscountBundleItemRequest request);
    }
}
using Simple.Ecommerce.Contracts.DiscountBundleItemContracts;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands
{
    public interface ICreateDiscountBundleItemDiscountCommand
    {
        Task<Result<DiscountBundleItemResponse>> Execute(DiscountBundleItemRequest request);
    }
}
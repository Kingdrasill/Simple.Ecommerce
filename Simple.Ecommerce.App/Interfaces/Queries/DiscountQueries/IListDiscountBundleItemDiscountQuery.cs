using Simple.Ecommerce.Contracts.DiscountBundleItemContracts;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Queries.DiscountQueries
{
    public interface IListDiscountBundleItemDiscountQuery
    {
        Task<Result<List<DiscountBundleItemResponse>>> Execute();
    }
}

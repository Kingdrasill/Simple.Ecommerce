using Simple.Ecommerce.Contracts.DiscountBundleItemContracts;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Queries.DiscountQueries
{
    public interface IListDiscountBundleItemDiscountQuery
    {
        Task<Result<List<DiscountBundleItemResponse>>> Execute();
    }
}

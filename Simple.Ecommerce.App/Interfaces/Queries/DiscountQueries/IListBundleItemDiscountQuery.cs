using Simple.Ecommerce.Contracts.DiscountBundleItemContracts;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Queries.DiscountQueries
{
    public interface IListBundleItemDiscountQuery
    {
        Task<Result<List<DiscountBundleItemResponse>>> Execute();
    }
}

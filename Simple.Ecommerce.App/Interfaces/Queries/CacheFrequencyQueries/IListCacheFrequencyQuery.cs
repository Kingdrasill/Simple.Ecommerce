using Simple.Ecommerce.Contracts.CacheFrequencyContracts;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Queries.CacheFrequencyQueries
{
    public interface IListCacheFrequencyQuery
    {
        Task<Result<List<CacheFrequencyResponse>>> Execute();
    }
}

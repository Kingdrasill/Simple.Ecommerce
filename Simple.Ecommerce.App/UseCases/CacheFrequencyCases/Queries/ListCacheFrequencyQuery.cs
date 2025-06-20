using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.CacheFrequencyQueries;
using Simple.Ecommerce.Contracts.CacheFrequencyContracts;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.UseCases.CacheFrequencyCases.Queries
{
    public class ListCacheFrequencyQuery : IListCacheFrequencyQuery
    {
        private readonly ICacheFrequencyRepository _repository;

        public ListCacheFrequencyQuery(
            ICacheFrequencyRepository repository
        )
        {
            _repository = repository;
        }

        public async Task<Result<List<CacheFrequencyResponse>>> Execute()
        {
            var listResult = await _repository.List();

            if (listResult.IsFailure)
            {
                return Result<List<CacheFrequencyResponse>>.Failure(listResult.Errors!);
            }

            var response = new List<CacheFrequencyResponse>();

            foreach (var frequency in listResult.GetValue())
            {
                response.Add(new CacheFrequencyResponse(
                    frequency.Id,
                    frequency.Entity,
                    frequency.Frequency,
                    frequency.HoursToLive,
                    frequency.Expirable,
                    frequency.KeepCached
                ));
            }

            return Result<List<CacheFrequencyResponse>>.Success(response);
        }
    }
}

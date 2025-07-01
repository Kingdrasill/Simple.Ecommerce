using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Queries.CacheFrequencyQueries;
using Simple.Ecommerce.App.Interfaces.Services.RepositoryHandler;
using Simple.Ecommerce.Contracts.CacheFrequencyContracts;
using Simple.Ecommerce.Domain.Entities.FrequencyEntity;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.UseCases.CacheFrequencyCases.Queries
{
    public class ListCacheFrequencyQuery : IListCacheFrequencyQuery
    {
        private readonly ICacheFrequencyRepository _repository;
        private readonly IRepositoryHandler _repositoryHandler;

        public ListCacheFrequencyQuery(
            ICacheFrequencyRepository repository,
            IRepositoryHandler repositoryHandler
        )
        {
            _repository = repository;
            _repositoryHandler = repositoryHandler;
        }

        public async Task<Result<List<CacheFrequencyResponse>>> Execute()
        {
            return await _repositoryHandler.ListFromRepository<CacheFrequency, CacheFrequencyResponse>(
                async () => await _repository.List(),
                frequency => new CacheFrequencyResponse(
                    frequency.Id,
                    frequency.Entity,
                    frequency.Frequency,
                    frequency.HoursToLive,
                    frequency.Expirable,
                    frequency.KeepCached
                )
            );
        }
    }
}

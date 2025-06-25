using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Services.RepositoryHandler
{
    public interface IRepositoryHandler
    {
        Task<Result<TEntity>> GetFromRepository<TEntity>(
            int id,
            Func<int, Task<Result<TEntity>>> getEntityFunc
        );
        Task<Result<TResponse>> GetFromRepository<TEntity, TResponse>(
            int id,
            Func<int, Task<Result<TEntity>>> getEntityFunc,
            Func<TEntity, TResponse> mapFunc
        );
        Task<Result<List<TEntity>>> GetFromRepositoryFromIds<TEntity, TId>(
            IEnumerable<TId> ids,
            Func<TId, Task<Result<TEntity>>> getByIdFunc
        );
        Task<Result<List<TEntity>>> GetFromRepositoryByIds<TEntity, TId>(
            IEnumerable<TId> ids,
            Func<IEnumerable<TId>, Task<Result<List<TEntity>>>> getByIdFunc
        );
        Task<Result<TResponse>> GetGroupedListFromRepository<TEntity, TInnerResponse, TResponse>(
            int id,
            Func<int, Task<Result<List<TEntity>>>> listFunc,
            Func<TEntity, TInnerResponse> mapItemFunc,
            Func<int, List<TInnerResponse>, TResponse> composeResponseFunc
        );
        Task<Result<List<TEntity>>> ListFromRepository<TEntity>(
            Func<Task<Result<List<TEntity>>>> listFunc
        );
        Task<Result<List<TResponse>>> ListFromRepository<TEntity, TResponse>(
            Func<Task<Result<List<TEntity>>>> listFunc,
            Func<TEntity, TResponse> mapFunc
        );
        Task<Result<List<TDomain>>> ListFromRepository<TDomain>(
            int filterId,
            Func<int, Task<Result<List<TDomain>>>> listFunc
        );
        Task<Result<List<TResponse>>> ListFromRepository<TDomain, TResponse>(
            int filterId,
            Func<int, Task<Result<List<TDomain>>>> listFunc,
            Func<TDomain, TResponse> mapFunc
        );
    }
}

using Simple.Ecommerce.App.Interfaces.Services.RepositoryHandler;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Services.RepositoryHandler
{
    public class RepositotyHandler : IRepositoryHandler
    {
        public async Task<Result<TEntity>> GetFromRepository<TEntity>(int id, Func<int, Task<Result<TEntity>>> getEntityFunc)
        {
            return await getEntityFunc(id);
        }

        public async Task<Result<TResponse>> GetFromRepository<TEntity, TResponse>(int id, Func<int, Task<Result<TEntity>>> getEntityFunc, Func<TEntity, TResponse> mapFunc)
        {
            var result = await getEntityFunc(id);
            if (result.IsFailure)
                return Result<TResponse>.Failure(result.Errors!);
            return Result<TResponse>.Success(mapFunc(result.GetValue()));
        }

        public async Task<Result<List<TEntity>>> GetFromRepositoryFromIds<TEntity, TId>(IEnumerable<TId> ids, Func<TId, Task<Result<TEntity>>> getByIdFunc)
        {
            var entities = new List<TEntity>();
            foreach (var id in ids)
            {
                var result = await getByIdFunc(id);
                if (result.IsFailure)
                    return Result<List<TEntity>>.Failure(result.Errors!);
                entities.Add(result.GetValue());
            }
            return Result<List<TEntity>>.Success(entities);
        }

        public async Task<Result<List<TEntity>>> GetFromRepositoryByIds<TEntity, TId>(IEnumerable<TId> ids, Func<IEnumerable<TId>, Task<Result<List<TEntity>>>> getByIdFunc)
        {
            return await getByIdFunc(ids);
        }

        public async Task<Result<TResponse>> GetGroupedListFromRepository<TEntity, TInnerResponse, TResponse>(int id, Func<int, Task<Result<List<TEntity>>>> listFunc, Func<TEntity, TInnerResponse> mapItemFunc, Func<int, List<TInnerResponse>, TResponse> composeResponseFunc)
        {
            var result = await listFunc(id);
            if (result.IsFailure)
                return Result<TResponse>.Failure(result.Errors!);
            var innerList = result.GetValue().Select(mapItemFunc).ToList();
            var response = composeResponseFunc(id, innerList);
            return Result<TResponse>.Success(response);
        }

        public async Task<Result<List<TEntity>>> ListFromRepository<TEntity>(Func<Task<Result<List<TEntity>>>> listFunc)
        {
            return await listFunc();
        }

        public async Task<Result<List<TResponse>>> ListFromRepository<TEntity, TResponse>(Func<Task<Result<List<TEntity>>>> listFunc, Func<TEntity, TResponse> mapFunc)
        {
            var result = await listFunc();
            if (result.IsFailure)
                return Result<List<TResponse>>.Failure(result.Errors!);
            return Result<List<TResponse>>.Success(result.GetValue().Select(mapFunc).ToList());
        }

        public async Task<Result<List<TDomain>>> ListFromRepository<TDomain>(int filterId, Func<int, Task<Result<List<TDomain>>>> listFunc)
        {
            return await listFunc(filterId);
        }

        public async Task<Result<List<TResponse>>> ListFromRepository<TDomain, TResponse>(int filterId, Func<int, Task<Result<List<TDomain>>>> listFunc, Func<TDomain, TResponse> mapFunc)
        {
            var result = await listFunc(filterId);
            if (result.IsFailure)
                return Result<List<TResponse>>.Failure(result.Errors!);
            return Result<List<TResponse>>.Success(result.GetValue().Select(mapFunc).ToList());
        }
    }
}

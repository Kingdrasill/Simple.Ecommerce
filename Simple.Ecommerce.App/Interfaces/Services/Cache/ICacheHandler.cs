using Simple.Ecommerce.Domain.Entities.FrequencyEntity;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Services.Cache
{
    public interface ICacheHandler
    {
        Result<TResponse> GetFromCache<TEntity, TResponse>(
            int id, 
            Func<IDictionary<string, object>, TResponse> factory
        );
        Result<TPrimaryResponse> GetFromCache<TEntity, TSecondaryResponse, TPrimaryResponse>(
            int id, 
            string secondaryPropName,
            Func<IDictionary<string, object>, string, TSecondaryResponse> secondaryFactory,
            Func<IDictionary<string, object>, TSecondaryResponse, TPrimaryResponse> primaryFactory
        );
        Result<TResponse> GetFromCacheByProperty<TEntity, TResponse>(
            string propName,
            object propValue,
            Func<IDictionary<string, object>, TResponse> factory
        );
        Result<List<TResponse>> ListFromCache<TEntity, TResponse>(
            Func<IDictionary<string, object>, TResponse> factroy
        );
        Result<List<TPrimaryResponse>> ListFromCache<TEntity, TSecondaryResponse, TPrimaryResponse>(
            string secondaryPropName,
            Func<IDictionary<string, object>, string, TSecondaryResponse> secondaryFactory,
            Func<IDictionary<string, object>, TSecondaryResponse, TPrimaryResponse> primaryFactory
        );
        Result<List<TResponse>> ListFromCacheByProperty<TEntity, TResponse>(
            string propName,
            object propValue,
            Func<IDictionary<string, object>, TResponse> factory
        );
        Result<List<TPrimaryResponse>> ListFromCacheByProperty<TEntity, TSecondaryResponse, TPrimaryResponse>(
            string propName, 
            object propValue, 
            string secondaryPropName, 
            Func<IDictionary<string, object>, string, TSecondaryResponse> secondaryFactory, 
            Func<IDictionary<string, object>, TSecondaryResponse, TPrimaryResponse> primaryFactory
        );
        Result<List<TResponse>> ListFromCacheByPropertyIn<TEntity, TResponse>(
            string propName, 
            IEnumerable<object> propValues, 
            Func<IDictionary<string, object>, 
            TResponse> factory
        );
        Task SendToCache<TEntity>() where TEntity : class;
        void SetItemStale<TEntity>();
    }
}

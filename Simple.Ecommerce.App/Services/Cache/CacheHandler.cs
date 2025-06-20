using Cache.Library.Core;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Data.BaseRepository;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.ServiceResolver;
using Simple.Ecommerce.Domain.ValueObjects.BaseObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Simple.Ecommerce.App.Services.Cache
{
    public class CacheHandler : ICacheHandler
    {
        private readonly ICache _cache;
        private readonly ICacheFrequencyRepository _cacheFrequencyRepository;
        private readonly IRepositoryResolver _repositoryResolver;

        public CacheHandler(
            ICache cache,
            ICacheFrequencyRepository cacheFrequencyRepository,
            IRepositoryResolver repositoryResolver
        )
        {
            _cache = cache;
            _cacheFrequencyRepository = cacheFrequencyRepository;
            _repositoryResolver = repositoryResolver;
        }

        public Result<TResponse> GetFromCache<TEntity, TResponse>(int id, Func<IDictionary<string, object>, TResponse> factory)
        {
            var entity = typeof(TEntity).Name;
            var getCache = _cache.GetItem(entity, id.ToString(), out _);
            if (getCache is null)
            {
                return Result<TResponse>.Failure(new());
            }
            var response = factory(getCache);
            return Result<TResponse>.Success(response);
        }

        public Result<TPrimaryResponse> GetFromCache<TEntity, TSecondaryResponse, TPrimaryResponse>(int id, string secondaryPropName, Func<IDictionary<string, object>, string, TSecondaryResponse> secondaryFactory, Func<IDictionary<string, object>, TSecondaryResponse, TPrimaryResponse> primaryFactory)
        {
            var entity = typeof(TEntity).Name;
            var getCache = _cache.GetItem(entity, id.ToString(), out _);
            if (getCache is null)
            {
                return Result<TPrimaryResponse>.Failure(new());
            }
            var nestedReponse = secondaryFactory(getCache, secondaryPropName);
            var response = primaryFactory(getCache, nestedReponse);
            return Result<TPrimaryResponse>.Success(response);
        }

        public Result<TResponse> GetFromCacheByProperty<TEntity, TResponse>(string propName, object propValue, Func<IDictionary<string, object>, TResponse> factory)
        {
            var entity = typeof(TEntity).Name;
            var getCache = _cache.GetItems(entity, out _);
            if (getCache is null)
            {
                return Result<TResponse>.Failure(new());
            }
            foreach (var item in getCache)
            {
                if (item.ContainsKey(propName) && Equals(item[propName], propValue))
                {
                    return Result<TResponse>.Success(factory(item));
                }
            }
            return Result<TResponse>.Failure(new()); ;
        }

        public Result<List<TResponse>> ListFromCache<TEntity, TResponse>(Func<IDictionary<string, object>, TResponse> factory)
        {
            var entity = typeof(TEntity).Name;
            var getCache = _cache.GetItems(entity, out _);
            if (getCache is null)
            {
                return Result<List<TResponse>>.Failure(new());
            }
            var response = getCache.Select(factory).ToList();
            return Result<List<TResponse>>.Success(response);
        }

        public Result<List<TPrimaryResponse>> ListFromCache<TEntity, TSecondaryResponse, TPrimaryResponse>(string secondaryPropName, Func<IDictionary<string, object>, string, TSecondaryResponse> secondaryFactory, Func<IDictionary<string, object>, TSecondaryResponse, TPrimaryResponse> primaryFactory)
        {
            var entity = typeof(TEntity).Name;
            var getCache = _cache.GetItems(entity, out _);
            if (getCache is null)
            {
                return Result<List<TPrimaryResponse>>.Failure(new());
            }
            var response = new List<TPrimaryResponse>();
            foreach (var item in getCache)
            {
                var nestedResponse = secondaryFactory(item, secondaryPropName);
                var primaryResponse = primaryFactory(item, nestedResponse);
                response.Add(primaryResponse);
            }
            return Result<List<TPrimaryResponse>>.Success(response);
        }

        public Result<List<TResponse>> ListFromCacheByProperty<TEntity, TResponse>(string propName, object propValue, Func<IDictionary<string, object>, TResponse> factory)
        {
            var entity = typeof(TEntity).Name;
            var getCache = _cache.GetItems(entity, out _);
            if (getCache is null)
            {
                return Result<List<TResponse>>.Failure(new());
            }
            var response = new List<TResponse>();
            foreach (var item in getCache)
            {
                if (item.ContainsKey(propName) && Equals(item[propName], propValue))
                {
                    response.Add(factory(item));
                }
            }
            return Result<List<TResponse>>.Success(response);
        }

        public Result<List<TPrimaryResponse>> ListFromCacheByProperty<TEntity, TSecondaryResponse, TPrimaryResponse>(string propName, object propValue, string secondaryPropName, Func<IDictionary<string, object>, string, TSecondaryResponse> secondaryFactory, Func<IDictionary<string, object>, TSecondaryResponse, TPrimaryResponse> primaryFactory)
        {
            var entity = typeof(TEntity).Name;
            var getCache = _cache.GetItems(entity, out _);
            if (getCache is null)
            {
                return Result<List<TPrimaryResponse>>.Failure(new());
            }
            var response = new List<TPrimaryResponse>();
            foreach (var item in getCache)
            {
                if (item.ContainsKey(propName) && Equals(item[propName], propValue))
                {
                    var nestedReponse = secondaryFactory(item, secondaryPropName);
                    var primaryResponse = primaryFactory(item, nestedReponse);
                    response.Add(primaryResponse);
                }
            }
            return Result<List<TPrimaryResponse>>.Success(response);
        }

        public Result<List<TResponse>> ListFromCacheByPropertyIn<TEntity, TResponse>(string propName, IEnumerable<object> propValues, Func<IDictionary<string, object>, TResponse> factory)
        {
            var entity = typeof(TEntity).Name;
            var getCache = _cache.GetItems(entity, out _);
            if (getCache is null)
            {
                return Result<List<TResponse>>.Failure(new());
            }

            var valuesSet = new HashSet<object>(propValues);
            var response = new List<TResponse>();

            foreach (var item in getCache)
            {
                if (item.TryGetValue(propName, out var value) && valuesSet.Contains(value))
                {
                    response.Add(factory(item));
                }
            }

            return Result<List<TResponse>>.Success(response);
        }

        public async Task SendToCache<TEntity>() where TEntity : class
        {
            var entity = typeof(TEntity).Name;
            var identifier = typeof(TEntity).GetProperties().First(p => p.GetCustomAttribute<KeyAttribute>() != null).Name;
            var getFreq = await _cacheFrequencyRepository.GetByEntity(entity);

            if (getFreq.IsFailure)
                return;

            var frequency = getFreq.GetValue();
            var repository = _repositoryResolver.GetRepository(typeof(TEntity)) as IBaseListRepository<TEntity>;
            if (repository is null)
                return;

            var listResult = await repository.List();
            if (listResult.IsFailure)
                return;

            var listDict = listResult.GetValue()
                .Select(item => {
                    var dict = new Dictionary<string, object>();

                    foreach (var prop in item!.GetType().GetProperties())
                    {
                        var value = prop.GetValue(item);

                        if (value is ValueObject vo)
                        {
                            foreach (var voProp in vo.GetType().GetProperties())
                            {
                                dict[$"{prop.Name}_{voProp.Name}"] = voProp.GetValue(vo)!;
                            }
                        }
                        else
                        {
                            dict[prop.Name] = value!;
                        }
                    }

                    item.GetType()
                    .GetProperties()
                    .ToDictionary(
                        prop => prop.Name,
                        prop => prop.GetValue(item)!
                    );

                    return dict;
                })
                .ToList();

            var result = _cache.AddItem(
                entity,
                identifier,
                listDict,
                false,
                frequency.HoursToLive,
                out var message,
                frequency.Expirable,
                frequency.KeepCached
            );

            if (!result)
            {
                Console.WriteLine("Erro ao incluir na cache: " + message);
            }
        }

        public void SetItemStale<TEntity>()
        {
            if (_cache.SetItemStale(typeof(TEntity).Name))
                Console.WriteLine("Item marcado como obsoleto!");
            else
                Console.WriteLine("item não está na cache!");
        }
    }
}

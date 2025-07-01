using Simple.Ecommerce.App.Interfaces.Data.BaseRepository;
using Simple.Ecommerce.Domain.Entities.FrequencyEntity;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Data
{
    public interface ICacheFrequencyRepository :
        IBaseCreateRepository<CacheFrequency>,
        IBaseUpdateRepository<CacheFrequency>,
        IBaseGetRepository<CacheFrequency>,
        IBaseListRepository<CacheFrequency>
    {
        Task<Result<CacheFrequency>> GetByEntity(string entity); 
    }
}

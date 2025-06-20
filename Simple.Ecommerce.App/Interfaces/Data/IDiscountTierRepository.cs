using Simple.Ecommerce.App.Interfaces.Data.BaseRepository;
using Simple.Ecommerce.Domain.Entities.DiscountTierEntity;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Data
{
    public interface IDiscountTierRepository : 
        IBaseCreateRepository<DiscountTier>,
        IBaseDeleteRepository<DiscountTier>,
        IBaseGetRepository<DiscountTier>,
        IBaseListRepository<DiscountTier>,
        IBaseUpdateRepository<DiscountTier>
    {
        Task<Result<List<DiscountTier>>> GetByDiscountId(int discountId);
    }
}

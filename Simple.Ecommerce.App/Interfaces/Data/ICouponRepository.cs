using Simple.Ecommerce.App.Interfaces.Data.BaseRepository;
using Simple.Ecommerce.Domain.Entities.CouponEntity;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Data
{
    public interface ICouponRepository : 
        IBaseCreateRepository<Coupon>,
        IBaseDeleteRepository<Coupon>,
        IBaseDetachRepository<Coupon>,
        IBaseGetRepository<Coupon>,
        IBaseListRepository<Coupon>,
        IBaseUpdateRepository<Coupon>
    {
        Task<Result<Coupon>> GetByCode(string code);
        Task<Result<List<Coupon>>> ListByCodes(List<string> codes);
        Task<Result<List<Coupon>>> ListByDiscountId(int discountId);
        Task<Result<List<Coupon>>> ListByIds(List<int> ids);
    }
}

using Simple.Ecommerce.App.Interfaces.Data.BaseRepository;
using Simple.Ecommerce.Domain.Entities.CouponEntity;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Data
{
    public interface ICouponRepository : 
        IBaseCreateRepository<Coupon>,
        IBaseDeleteRepository<Coupon>,
        IBaseGetRepository<Coupon>,
        IBaseListRepository<Coupon>,
        IBaseUpdateRepository<Coupon>
    {
        Task<Result<List<Coupon>>> GetByDiscountId(int discountId);
        Task<Result<Coupon>> GetByCode(string code);
    }
}

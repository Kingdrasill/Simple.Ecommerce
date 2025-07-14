using Simple.Ecommerce.App.Interfaces.Data.BaseRepository;
using Simple.Ecommerce.Domain.Entities.DiscountBundleItemEntity;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Data
{
    public interface IDiscountBundleItemRepository :
        IBaseCreateRepository<DiscountBundleItem>,
        IBaseDeleteRepository<DiscountBundleItem>,
        IBaseGetRepository<DiscountBundleItem>,
        IBaseListRepository<DiscountBundleItem>,
        IBaseUpdateRepository<DiscountBundleItem>
    {
        Task<Result<List<DiscountBundleItem>>> GetByDiscountId(int discountId);
        Task<Result<List<DiscountBundleItem>>> GetByProductId(int productId);
        Task<Result<List<int>>> GetProductIdsByDiscountId(int discountId);
    }
}

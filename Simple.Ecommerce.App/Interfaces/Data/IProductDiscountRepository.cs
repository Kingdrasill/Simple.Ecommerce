using Simple.Ecommerce.App.Interfaces.Data.BaseRepository;
using Simple.Ecommerce.Domain.Entities.ProductDiscountEntity;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Data
{
    public interface IProductDiscountRepository :
        IBaseCreateRepository<ProductDiscount>,
        IBaseDeleteRepository<ProductDiscount>,
        IBaseDetachRepository<ProductDiscount>,
        IBaseGetRepository<ProductDiscount>,
        IBaseListRepository<ProductDiscount>
    {
        Task<Result<List<ProductDiscount>>> GetProductDiscountsByIds(List<int> ids);
        Task<Result<List<ProductDiscount>>> GetByProductId(int productId);
        Task<Result<List<ProductDiscount>>> GetByDiscountId(int discountId);
        Task<Result<ProductDiscount>> GetByProductIdDiscountId(int productId, int discountId);
        Task<Result<List<ProductDiscount>>> GetByProductIdsDiscountIds(List<(int productId, int discountId)> productsDiscountsIds);
    }
}

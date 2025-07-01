using Simple.Ecommerce.App.Interfaces.Data.BaseRepository;
using Simple.Ecommerce.Domain.Entities.ProductDiscountEntity;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Data
{
    public interface IProductDiscountRepository :
        IBaseCreateRepository<ProductDiscount>,
        IBaseDeleteRepository<ProductDiscount>,
        IBaseGetRepository<ProductDiscount>,
        IBaseListRepository<ProductDiscount>
    {
        Task<Result<List<ProductDiscount>>> GetByProductId(int productId);
        Task<Result<List<ProductDiscount>>> GetByDiscountId(int discountId);
    }
}

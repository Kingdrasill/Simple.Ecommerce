using Simple.Ecommerce.App.Interfaces.Data.BaseRepository;
using Simple.Ecommerce.Domain.Entities.ProductCategoryEntity;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Data
{
    public interface IProductCategoryRepository : 
        IBaseCreateRepository<ProductCategory>,
        IBaseDeleteRepository<ProductCategory>,
        IBaseGetRepository<ProductCategory>,
        IBaseListRepository<ProductCategory>
    {
        Task<Result<List<ProductCategory>>> GetByProductId(int productId);
        Task<Result<List<ProductCategory>>> GetByCategoryId(int categoryId);
    }
}

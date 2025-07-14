using Simple.Ecommerce.App.Interfaces.Data.BaseRepository;
using Simple.Ecommerce.Domain.Entities.ProductPhotoEntity;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Data
{
    public interface IProductPhotoRepository :
        IBaseCreateRepository<ProductPhoto>,
        IBaseDeleteRepository<ProductPhoto>,
        IBaseGetRepository<ProductPhoto>,
        IBaseListRepository<ProductPhoto>
    {
        Task<Result<List<ProductPhoto>>> ListByProductId(int productId);
        Task<Result<List<ProductPhoto>>> GetByImageNames(List<string> imageNames);
    }
}

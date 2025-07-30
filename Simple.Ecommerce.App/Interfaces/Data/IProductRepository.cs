using Simple.Ecommerce.App.Interfaces.Data.BaseRepository;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.ProductEntity;

namespace Simple.Ecommerce.App.Interfaces.Data
{
    public interface IProductRepository :
        IBaseCreateRepository<Product>,
        IBaseDeleteRepository<Product>,
        IBaseDetachRepository<Product>,
        IBaseGetRepository<Product>,
        IBaseListRepository<Product>,
        IBaseUpdateRepository<Product>
    {
        Task<Result<List<Product>>> GetProductsByIds(List<int> ids);
    }
}

using Simple.Ecommerce.App.Interfaces.Data.BaseRepository;
using Simple.Ecommerce.Domain.Entities.ProductEntity;

namespace Simple.Ecommerce.App.Interfaces.Data
{
    public interface IProductRepository :
        IBaseCreateRepository<Product>,
        IBaseDeleteRepository<Product>,
        IBaseGetRepository<Product>,
        IBaseListRepository<Product>,
        IBaseUpdateRepository<Product>
    {
    }
}

using Simple.Ecommerce.App.Interfaces.Data.BaseRepository;
using Simple.Ecommerce.Domain.Entities.CategoryEntity;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Data
{
    public interface ICategoryRepository :
        IBaseCreateRepository<Category>,
        IBaseDeleteRepository<Category>,
        IBaseGetRepository<Category>,
        IBaseListRepository<Category>,
        IBaseUpdateRepository<Category>
    {
        Task<Result<List<Category>>> GetCategoriesByIds(List<int> ids);
    }
}

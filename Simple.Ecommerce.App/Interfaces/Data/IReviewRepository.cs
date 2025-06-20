using Simple.Ecommerce.App.Interfaces.Data.BaseRepository;
using Simple.Ecommerce.Domain.Entities.ReviewEntity;

namespace Simple.Ecommerce.App.Interfaces.Data
{
    public interface IReviewRepository :
        IBaseCreateRepository<Review>,
        IBaseDeleteRepository<Review>,
        IBaseUpdateRepository<Review>,
        IBaseGetRepository<Review>,
        IBaseListRepository<Review>
    {
    }
}

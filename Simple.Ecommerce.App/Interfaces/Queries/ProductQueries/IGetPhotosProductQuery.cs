using Simple.Ecommerce.Contracts.ProductPhotoContracts;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Queries.ProductQueries
{
    public interface IGetPhotosProductQuery
    {
        Task<Result<ProductPhotosResponse>> Execute(int productId);
    }
}

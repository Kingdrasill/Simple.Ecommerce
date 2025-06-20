using Simple.Ecommerce.Contracts.ProductPhotoContracts;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Queries.ProductQueries
{
    public interface IGetPhotosProductQuery
    {
        Task<Result<ProductPhotosResponse>> Execute(int productId);
    }
}

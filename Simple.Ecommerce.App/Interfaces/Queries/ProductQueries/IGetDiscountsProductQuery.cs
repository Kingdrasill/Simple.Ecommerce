using Simple.Ecommerce.Contracts.ProductDiscountContracts;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Queries.ProductQueries
{
    public interface IGetDiscountsProductQuery
    {
        Task<Result<List<ProductDiscountDTO>>> Execute(int productId);
    }
}

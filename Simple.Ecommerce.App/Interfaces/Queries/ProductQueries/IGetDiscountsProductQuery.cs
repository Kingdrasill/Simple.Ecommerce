using Simple.Ecommerce.Contracts.ProductDiscountContracts;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Queries.ProductQueries
{
    public interface IGetDiscountsProductQuery
    {
        Task<Result<List<ProductDiscountDTO>>> Execute(int productId);
    }
}

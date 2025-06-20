using Simple.Ecommerce.Contracts.OrderDiscountContracts;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Queries.OrderQueries
{
    public interface IGetDiscountsOrderQuery
    {
        Task<Result<List<OrderDiscountDTO>>> Execute(int orderId);
    }
}

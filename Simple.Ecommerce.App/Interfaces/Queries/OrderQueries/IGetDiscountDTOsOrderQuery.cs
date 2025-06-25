using Simple.Ecommerce.Contracts.OrderDiscountContracts;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Queries.OrderQueries
{
    public interface IGetDiscountDTOsOrderQuery
    {
        Task<Result<List<OrderDiscountDTO>>> Execute(int orderId);
    }
}

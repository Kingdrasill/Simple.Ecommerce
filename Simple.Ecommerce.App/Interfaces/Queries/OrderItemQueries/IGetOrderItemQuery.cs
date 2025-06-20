using Simple.Ecommerce.Contracts.OrderItemContracts;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Queries.OrderItemQueries
{
    public interface IGetOrderItemQuery
    {
        Task<Result<OrderItemResponse>> Execute(int id, bool NoTracking = true);
    }
}

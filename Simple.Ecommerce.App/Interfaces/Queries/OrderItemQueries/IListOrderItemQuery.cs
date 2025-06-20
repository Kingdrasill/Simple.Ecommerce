using Simple.Ecommerce.Contracts.OrderItemContracts;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Queries.OrderItemQueries
{
    public interface IListOrderItemQuery
    {
        Task<Result<List<OrderItemResponse>>> Execute();
    }
}

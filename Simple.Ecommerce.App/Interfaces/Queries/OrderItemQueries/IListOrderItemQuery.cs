using Simple.Ecommerce.Contracts.OrderItemContracts;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Queries.OrderItemQueries
{
    public interface IListOrderItemQuery
    {
        Task<Result<List<OrderItemResponse>>> Execute();
    }
}

using Simple.Ecommerce.Contracts.OrderItemContracts;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Queries.OrderItemQueries
{
    public interface IListOrderItemQuery
    {
        Task<Result<List<OrderItemResponse>>> Execute();
    }
}

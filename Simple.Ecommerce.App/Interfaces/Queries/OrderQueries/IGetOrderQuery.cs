using Simple.Ecommerce.Contracts.OrderContracts;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Queries.OrderQueries
{
    public interface IGetOrderQuery
    {
        Task<Result<OrderResponse>> Execute(int id);
    }
}

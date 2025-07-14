using Simple.Ecommerce.Contracts.OrderContracts;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Queries.OrderQueries
{
    public interface IGetPaymentMethodOrderQuery
    {
        Task<Result<OrderPaymentMethodResponse>> Execute(int orderId);
    }
}

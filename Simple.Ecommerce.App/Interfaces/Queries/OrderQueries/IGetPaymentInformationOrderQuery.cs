using Simple.Ecommerce.Contracts.OrderContracts.PaymentInformations;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Queries.OrderQueries
{
    public interface IGetPaymentInformationOrderQuery
    {
        Task<Result<OrderPaymentInformationResponse>> Execute(int orderId);
    }
}

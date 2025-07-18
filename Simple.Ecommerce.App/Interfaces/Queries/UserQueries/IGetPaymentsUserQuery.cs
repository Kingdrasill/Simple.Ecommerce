using Simple.Ecommerce.Contracts.UserPaymentContracts;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Queries.UserQueries
{
    public interface IGetPaymentsUserQuery
    {
        Task<Result<UserPaymentsResponse>> Execute(int userId);
    }
}

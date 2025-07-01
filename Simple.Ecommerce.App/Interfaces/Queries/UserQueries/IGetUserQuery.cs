using Simple.Ecommerce.Contracts.UserContracts;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Queries.UserQueries
{
    public interface IGetUserQuery
    {
        Task<Result<UserResponse>> Execute(int id);
    }
}

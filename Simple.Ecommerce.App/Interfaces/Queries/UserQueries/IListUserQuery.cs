using Simple.Ecommerce.Contracts.UserContracts;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Queries.UserQueries
{
    public interface IListUserQuery
    {
        Task<Result<List<UserResponse>>> Execute();
    }
}

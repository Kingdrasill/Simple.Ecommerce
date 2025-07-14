using Simple.Ecommerce.Contracts.UserContracts;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Queries.UserQueries
{
    public interface IListUserQuery
    {
        Task<Result<List<UserResponse>>> Execute();
    }
}

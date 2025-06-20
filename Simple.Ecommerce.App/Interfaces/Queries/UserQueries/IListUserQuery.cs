using Simple.Ecommerce.Contracts.UserContracts;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Queries.UserQueries
{
    public interface IListUserQuery
    {
        Task<Result<List<UserResponse>>> Execute();
    }
}

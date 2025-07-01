using Simple.Ecommerce.Contracts.UserAddressContracts;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Queries.UserQueries
{
    public interface IGetAddressesUserQuery
    {
        Task<Result<UserAddressesResponse>> Execute(int userId);
    }
}

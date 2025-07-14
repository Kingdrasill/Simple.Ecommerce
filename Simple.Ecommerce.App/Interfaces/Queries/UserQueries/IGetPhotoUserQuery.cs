using Simple.Ecommerce.Contracts.UserPhotoContracts;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Queries.UserQueries
{
    public interface IGetPhotoUserQuery
    {
        Task<Result<UserPhotoResponse>> Execute(int userId);
    }
}

using Simple.Ecommerce.Contracts.UserPhotoContracts;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Queries.UserQueries
{
    public interface IGetPhotoUserQuery
    {
        Task<Result<UserPhotoResponse>> Execute(int userId);
    }
}

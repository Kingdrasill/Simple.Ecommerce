using Simple.Ecommerce.Contracts.UserCardContracts;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Queries.UserQueries
{
    public interface IGetCardsUserQuery
    {
        Task<Result<UserCardsReponse>> Execute(int userId);
    }
}

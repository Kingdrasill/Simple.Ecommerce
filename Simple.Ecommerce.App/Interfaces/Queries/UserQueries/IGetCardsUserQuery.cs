using Simple.Ecommerce.Contracts.UserCardContracts;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Queries.UserQueries
{
    public interface IGetCardsUserQuery
    {
        Task<Result<UserCardsReponse>> Execute(int userId);
    }
}

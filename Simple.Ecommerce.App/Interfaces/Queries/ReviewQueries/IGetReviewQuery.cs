using Simple.Ecommerce.Contracts.ReviewContracts;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Queries.ReviewQueries
{
    public interface IGetReviewQuery
    {
        Task<Result<ReviewResponse>> Execute(int id);
    }
}

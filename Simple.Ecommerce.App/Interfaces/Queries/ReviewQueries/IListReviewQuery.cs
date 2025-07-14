using Simple.Ecommerce.Contracts.ReviewContracts;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Queries.ReviewQueries
{
    public interface IListReviewQuery
    {
        Task<Result<List<ReviewResponse>>> Execute();
    }
}

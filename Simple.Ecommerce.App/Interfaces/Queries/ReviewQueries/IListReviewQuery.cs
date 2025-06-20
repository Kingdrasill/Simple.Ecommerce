using Simple.Ecommerce.Contracts.ReviewContracts;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Queries.ReviewQueries
{
    public interface IListReviewQuery
    {
        Task<Result<List<ReviewResponse>>> Execute();
    }
}

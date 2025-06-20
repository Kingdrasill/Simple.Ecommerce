using Simple.Ecommerce.Contracts.ReviewContracts;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Queries.ReviewQueries
{
    public interface IGetReviewQuery
    {
        Task<Result<ReviewResponse>> Execute(int id, bool NoTracking = true);
    }
}

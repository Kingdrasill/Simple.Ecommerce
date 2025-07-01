using Simple.Ecommerce.Contracts.ReviewContracts;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Commands.ReviewCommands
{
    public interface IUpdateReviewCommand
    {
        Task<Result<ReviewResponse>> Execute(ReviewRequest request);
    }
}

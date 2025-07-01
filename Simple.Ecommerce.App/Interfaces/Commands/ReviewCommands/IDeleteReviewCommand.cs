using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Commands.ReviewCommands
{
    public interface IDeleteReviewCommand
    {
        Task<Result<bool>> Execute(int id);
    }
}

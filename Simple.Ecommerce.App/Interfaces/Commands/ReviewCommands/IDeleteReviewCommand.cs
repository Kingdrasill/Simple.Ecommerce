using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Commands.ReviewCommands
{
    public interface IDeleteReviewCommand
    {
        Task<Result<bool>> Execute(int id);
    }
}

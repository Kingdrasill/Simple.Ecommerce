using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Commands.ReviewCommands
{
    public interface IDeleteReviewCommand
    {
        Task<Result<bool>> Execute(int id);
    }
}

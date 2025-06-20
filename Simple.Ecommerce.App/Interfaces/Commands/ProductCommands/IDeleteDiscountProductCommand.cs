using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Commands.ProductCommands
{
    public interface IDeleteDiscountProductCommand
    {
        Task<Result<bool>> Execute(int productDiscountId);
    }
}

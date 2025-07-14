using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Commands.ProductCommands
{
    public interface IRemoveDiscountProductCommand
    {
        Task<Result<bool>> Execute(int productDiscountId);
    }
}

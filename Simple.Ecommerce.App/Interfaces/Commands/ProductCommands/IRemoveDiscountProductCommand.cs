using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Commands.ProductCommands
{
    public interface IRemoveDiscountProductCommand
    {
        Task<Result<bool>> Execute(int productDiscountId);
    }
}

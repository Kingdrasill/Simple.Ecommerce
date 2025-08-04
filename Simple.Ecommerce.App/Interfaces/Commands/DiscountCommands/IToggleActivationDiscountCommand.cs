using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands
{
    public interface IToggleActivationDiscountCommand
    {
        Task<Result<bool>> Execute(int id, bool isActive);
    }
}

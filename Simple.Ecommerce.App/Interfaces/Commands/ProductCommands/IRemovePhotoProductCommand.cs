using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Commands.ProductCommands
{
    public interface IRemovePhotoProductCommand
    {
        Task<Result<bool>> Execute(int productPhotoId);
    }
}

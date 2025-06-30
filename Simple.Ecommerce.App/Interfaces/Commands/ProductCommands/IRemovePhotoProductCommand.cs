using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Commands.ProductCommands
{
    public interface IRemovePhotoProductCommand
    {
        Task<Result<bool>> Execute(int productPhotoId);
    }
}

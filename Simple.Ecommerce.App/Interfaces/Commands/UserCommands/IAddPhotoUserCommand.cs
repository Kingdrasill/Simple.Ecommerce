using Simple.Ecommerce.Contracts.UserPhotoContracts;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Commands.UserCommands
{
    public interface IAddPhotoUserCommand
    {
        Task<Result<UserPhotoResponse>> Execute(UserPhotoRequest request, Stream stream, string fileExtension);
    }
}

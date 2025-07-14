using Simple.Ecommerce.Contracts.ProductPhotoContracts;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Commands.ProductCommands
{
    public interface IAddPhotoProductCommand
    {
        Task<Result<ProductPhotoResponse>> Execute(ProductPhotoRequest request, Stream stream, string fileExtension);
    }
}

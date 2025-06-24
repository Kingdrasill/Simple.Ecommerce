using Simple.Ecommerce.Contracts.PhotoContracts;

namespace Simple.Ecommerce.Contracts.ProductPhotoContracts
{
    public record ProductPhotosResponse
    (
        int ProductId,
        List<PhotoProductResponse> Photo
    );
}

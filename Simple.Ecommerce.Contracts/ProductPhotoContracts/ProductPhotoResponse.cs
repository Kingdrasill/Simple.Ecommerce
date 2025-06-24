using Simple.Ecommerce.Contracts.PhotoContracts;

namespace Simple.Ecommerce.Contracts.ProductPhotoContracts
{
    public record ProductPhotoResponse
    (
        PhotoProductResponse Photo,
        int ProductId
    );
}

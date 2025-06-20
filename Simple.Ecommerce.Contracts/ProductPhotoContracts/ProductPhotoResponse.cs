using Simple.Ecommerce.Contracts.PhotoContracts;

namespace Simple.Ecommerce.Contracts.ProductPhotoContracts
{
    public record ProductPhotoResponse
    (
        PhotoResponse Photo,
        int ProductId
    );
}

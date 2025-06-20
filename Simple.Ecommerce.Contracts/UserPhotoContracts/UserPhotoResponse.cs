using Simple.Ecommerce.Contracts.PhotoContracts;

namespace Simple.Ecommerce.Contracts.UserPhotoContracts
{
    public record UserPhotoResponse
    (
        int Id,
        PhotoResponse? Photo
    );
}

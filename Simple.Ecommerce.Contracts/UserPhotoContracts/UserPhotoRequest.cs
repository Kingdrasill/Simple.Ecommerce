namespace Simple.Ecommerce.Contracts.UserPhotoContracts
{
    public record UserPhotoRequest
    (
        int Id,
        bool Compress = true,
        bool Deletable = true
    );
}

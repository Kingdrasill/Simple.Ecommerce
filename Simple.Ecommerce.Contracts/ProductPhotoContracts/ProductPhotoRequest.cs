namespace Simple.Ecommerce.Contracts.ProductPhotoContracts
{
    public record ProductPhotoRequest
    (
        int ProductId,
        bool Compress = true,
        bool Deletable = true
    );
}

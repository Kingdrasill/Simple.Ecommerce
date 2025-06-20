namespace Simple.Ecommerce.Contracts.PhotoContracts
{
    public record PhotoResponse
    (
        string FileName,
        int? Id = null
    );
}

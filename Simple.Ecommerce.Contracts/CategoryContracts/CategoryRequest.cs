namespace Simple.Ecommerce.Contracts.CategoryContracts
{
    public record CategoryRequest
    (
        string Name,
        int Id = 0
    );
}

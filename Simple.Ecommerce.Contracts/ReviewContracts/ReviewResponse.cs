namespace Simple.Ecommerce.Contracts.ReviewContracts
{
    public record ReviewResponse
    (
        int Id,
        int Score,
        string? Comment,
        int UserId,
        int ProductId
    );
}

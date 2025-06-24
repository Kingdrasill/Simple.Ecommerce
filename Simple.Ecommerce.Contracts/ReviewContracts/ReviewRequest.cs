namespace Simple.Ecommerce.Contracts.ReviewContracts
{
    public record ReviewRequest
    (
        int Score,
        string? Comment,
        int UserId,
        int ProductId,
        int Id = 0
    );
}

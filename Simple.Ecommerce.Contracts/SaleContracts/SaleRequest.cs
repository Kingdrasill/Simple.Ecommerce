namespace Simple.Ecommerce.Contracts.SaleContracts
{
    public record SaleRequest
    (
        string Name,
        int Percentage,
        int ProductId,
        string Status,
        DateTime DateStart,
        DateTime DateEnd,
        int Id = 0
    );
}

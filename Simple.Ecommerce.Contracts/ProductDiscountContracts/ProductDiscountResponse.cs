namespace Simple.Ecommerce.Contracts.ProductDiscountContracts
{
    public record ProductDiscountResponse
    (
        int Id,
        int ProductId,
        int DiscountId
    );
}

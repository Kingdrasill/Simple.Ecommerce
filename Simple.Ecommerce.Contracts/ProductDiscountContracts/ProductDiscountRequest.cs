namespace Simple.Ecommerce.Contracts.ProductDiscountContracts
{
    public record ProductDiscountRequest
    (
        int ProductId,
        int DiscountId
    );
}

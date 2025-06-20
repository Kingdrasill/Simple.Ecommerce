namespace Simple.Ecommerce.Contracts.DiscountBundleItemContracts
{
    public record DiscountBundleItemRequest
    (
        int ProductId,
        int Quantity,
        int DiscountId, 
        int Id = 0
    );
}

namespace Simple.Ecommerce.Contracts.DiscountBundleItemContracts
{
    public record DiscountBundleItemResponse
    (
        int Id,
        int ProductId,
        int Quantity,
        int? DiscountId = null
    );
}

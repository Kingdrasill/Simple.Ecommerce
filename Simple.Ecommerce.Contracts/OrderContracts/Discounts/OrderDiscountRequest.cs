namespace Simple.Ecommerce.Contracts.OrderContracts.Discounts
{
    public record OrderDiscountRequest
    (
        int OrderId,
        int? DiscountId = null
    );
}

namespace Simple.Ecommerce.Contracts.OrderContracts
{
    public record OrderDiscountRequest
    (
        int OrderId,
        int? DiscountId = null
    );
}

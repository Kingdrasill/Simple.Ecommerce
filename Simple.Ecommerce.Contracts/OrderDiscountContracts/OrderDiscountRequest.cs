namespace Simple.Ecommerce.Contracts.OrderDiscountContracts
{
    public record OrderDiscountRequest
    (
        int OrderId,
        int DiscountId
    );
}

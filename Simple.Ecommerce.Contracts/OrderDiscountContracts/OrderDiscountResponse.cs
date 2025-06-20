namespace Simple.Ecommerce.Contracts.OrderDiscountContracts
{
    public record OrderDiscountResponse
    (
        int Id,
        int OrderId,
        int DiscountId
    );
}

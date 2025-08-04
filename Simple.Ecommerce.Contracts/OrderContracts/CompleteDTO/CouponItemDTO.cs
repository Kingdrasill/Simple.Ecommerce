namespace Simple.Ecommerce.Contracts.OrderContracts.CompleteDTO
{
    public record CouponItemDTO
    (
        int Id,
        int DiscountId,
        string Code
    );
}

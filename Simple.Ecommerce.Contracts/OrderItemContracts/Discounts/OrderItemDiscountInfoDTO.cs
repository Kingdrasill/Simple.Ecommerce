using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Contracts.OrderItemContracts.Discounts
{
    public record OrderItemDiscountInfoDTO
    (
        int OrderItemId,
        int ProductId,
        int? DiscountId,
        DiscountType? DiscountType
    );
}

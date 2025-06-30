using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Contracts.OrderItemContracts
{
    public record OrderItemDiscountInfoDTO
    (
        int OrderItemId,
        int ProductId,
        int? DiscountId,
        DiscountType? DiscountType
    );
}

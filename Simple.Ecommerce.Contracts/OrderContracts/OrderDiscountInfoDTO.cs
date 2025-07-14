using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Contracts.OrderContracts
{
    public record OrderDiscountInfoDTO
    (
        int orderId,
        int ProductId,
        int? DiscountId,
        DiscountType? DiscountType
    );
}

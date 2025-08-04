using Simple.Ecommerce.Contracts.CouponContracts;
using Simple.Ecommerce.Contracts.DiscountContracts;

namespace Simple.Ecommerce.Contracts.OrderItemContracts.Discounts
{
    public record OrderItemInfoDTO
    (
        int Id,
        int ProductId,
        DiscountInfoDTO? Discount,
        CouponInfoDTO? Coupon
    );
}

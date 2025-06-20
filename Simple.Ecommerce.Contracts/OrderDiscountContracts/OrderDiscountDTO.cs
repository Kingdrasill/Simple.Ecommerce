using Simple.Ecommerce.Contracts.DiscountContracts;

namespace Simple.Ecommerce.Contracts.OrderDiscountContracts
{
    public record OrderDiscountDTO
    (
        int OrderDiscountId,
        DiscountDTO discount
    );
}

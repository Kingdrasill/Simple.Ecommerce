using Simple.Ecommerce.Contracts.DiscountContracts;

namespace Simple.Ecommerce.Contracts.OrderContracts.Discounts
{
    public record OrderDiscountDTO
    (
        int Id,
        DiscountDTO? Discount
    );
}

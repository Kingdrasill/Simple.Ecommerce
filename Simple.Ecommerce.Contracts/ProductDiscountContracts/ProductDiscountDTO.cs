using Simple.Ecommerce.Contracts.DiscountContracts;

namespace Simple.Ecommerce.Contracts.ProductDiscountContracts
{
    public record ProductDiscountDTO
    (
        int ProductDiscountId,
        DiscountCompleteDTO discount
    );
}

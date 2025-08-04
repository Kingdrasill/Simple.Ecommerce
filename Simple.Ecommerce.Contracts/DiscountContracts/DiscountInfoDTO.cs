using Simple.Ecommerce.Domain.Enums.Discount;

namespace Simple.Ecommerce.Contracts.DiscountContracts
{
    public record DiscountInfoDTO
    (
        int Id,
        string Name,
        DiscountType DiscountType
    );
}

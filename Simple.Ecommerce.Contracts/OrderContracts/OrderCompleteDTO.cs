using Simple.Ecommerce.Contracts.AddressContracts;
using Simple.Ecommerce.Contracts.DiscountContracts;
using Simple.Ecommerce.Contracts.OrderItemContracts;
using Simple.Ecommerce.Domain.Enums.OrderType;
using Simple.Ecommerce.Domain.Enums.PaymentMethod;

namespace Simple.Ecommerce.Contracts.OrderContracts
{
    public record OrderCompleteDTO
    (
        int Id,
        int UserId,
        string UserName,
        OrderType OrderType,
        OrderAddressResponse Address,
        PaymentMethod? PaymentMethod,
        decimal? TotalPrice,
        DateTime? OrderDate,
        bool Confirmation,
        string Status,
        DiscountItemDTO? AppliedDiscount,
        List<OrderItemDTO> Items,
        List<BundleItemsDTO> BundledItems
    );
}

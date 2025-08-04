using Simple.Ecommerce.Contracts.AddressContracts;
using Simple.Ecommerce.Contracts.OrderItemContracts;
using Simple.Ecommerce.Contracts.PaymentInformationContracts;
using Simple.Ecommerce.Domain.Enums.OrderType;

namespace Simple.Ecommerce.Contracts.OrderContracts.CompleteDTO
{
    public record OrderCompleteDTO
    (
        int Id,
        int UserId,
        string UserName,
        OrderType OrderType,
        OrderAddressResponse Address,
        PaymentInformationOrderResponse? PaymentInformation,
        decimal? TotalPrice,
        DateTime? OrderDate,
        bool Confirmation,
        string Status,
        DiscountItemDTO? AppliedDiscount,
        List<OrderItemDTO> Items,
        List<BundleItemsDTO> BundledItems
    );
}

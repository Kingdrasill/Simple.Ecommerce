using Simple.Ecommerce.Contracts.AddressContracts;
using Simple.Ecommerce.Contracts.PaymentInformationContracts;
using Simple.Ecommerce.Domain.Enums.OrderType;

namespace Simple.Ecommerce.Contracts.OrderContracts
{
    public record OrderResponse
    (
        int Id,
        int UserId,
        OrderType OrderType,
        OrderAddressResponse Address,
        PaymentInformationOrderResponse? PaymentInformation,
        decimal? TotalPrice,
        DateTime? OrderDate,
        bool Confirmation,
        string Status,
        int? DiscountId
    );
}

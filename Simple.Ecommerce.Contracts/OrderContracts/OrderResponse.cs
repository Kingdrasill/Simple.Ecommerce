using Simple.Ecommerce.Contracts.AddressContracts;
using Simple.Ecommerce.Domain.Enums.OrderType;
using Simple.Ecommerce.Domain.Enums.PaymentMethod;

namespace Simple.Ecommerce.Contracts.OrderContracts
{
    public record OrderResponse
    (
        int Id,
        int UserId,
        OrderType OrderType,
        OrderAddressResponse Address,
        PaymentMethod? PaymentMethod,
        decimal? TotalPrice,
        DateTime? OrderDate,
        bool Confirmation,
        string Status
    );
}

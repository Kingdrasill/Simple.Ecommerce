using Simple.Ecommerce.Contracts.AddressContracts;
using Simple.Ecommerce.Domain.Enums.OrderType;

namespace Simple.Ecommerce.Contracts.OrderContracts
{
    public record OrderResponse
    (
        int Id,
        DateTime OrderDate,
        int UserId,
        decimal TotalPrice,
        OrderType OrderType,
        bool Confirmation,
        string Status,
        string? PaymentMethod,
        AddressResponse Address
    );
}

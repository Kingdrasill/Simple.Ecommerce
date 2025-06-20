using Simple.Ecommerce.Contracts.AddressContracts;
using Simple.Ecommerce.Domain.Enums.OrderType;

namespace Simple.Ecommerce.Contracts.OrderContracts
{
    public record OrderRequest
    (
        DateTime OrderDate,
        int UserId,
        decimal TotalPrice,
        OrderType OrderType,
        string? PaymentMethod,
        AddressRequest Address,
        int Id = 0
    );
}

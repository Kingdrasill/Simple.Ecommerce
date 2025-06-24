using Simple.Ecommerce.Contracts.AddressContracts;
using Simple.Ecommerce.Domain.Enums.OrderType;
using Simple.Ecommerce.Domain.Enums.PaymentMethod;

namespace Simple.Ecommerce.Contracts.OrderContracts
{
    public record OrderRequest
    (
        int UserId,
        OrderType OrderType,
        AddressRequest Address,
        PaymentMethod? PaymentMethod = null,
        decimal? TotalPrice = null,
        DateTime? OrderDate = null,
        int Id = 0
    );
}

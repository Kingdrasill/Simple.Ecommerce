using Simple.Ecommerce.Contracts.AddressContracts;
using Simple.Ecommerce.Contracts.PaymentInformationContracts;
using Simple.Ecommerce.Domain.Enums.OrderType;

namespace Simple.Ecommerce.Contracts.OrderContracts
{
    public record OrderRequest
    (
        int UserId,
        OrderType OrderType,
        AddressRequest Address,
        PaymentInformationRequest? PaymentInformation = null,
        decimal? TotalPrice = null,
        DateTime? OrderDate = null,
        int? DiscountId = null,
        int Id = 0
    );
}

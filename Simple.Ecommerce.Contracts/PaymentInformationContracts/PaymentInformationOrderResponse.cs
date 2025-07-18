using Simple.Ecommerce.Domain.Enums.CardFlag;
using Simple.Ecommerce.Domain.Enums.PaymentMethod;

namespace Simple.Ecommerce.Contracts.PaymentInformationContracts
{
    public record PaymentInformationOrderResponse
    (
        PaymentMethod PaymentMethod,
        string? PaymentName,
        string? PaymentKey,
        string? ExpirationMonth,
        string? ExpirationYear,
        CardFlag? CardFlag,
        string? Last4Digits
    );
}

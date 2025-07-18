using Simple.Ecommerce.Domain.Enums.PaymentMethod;

namespace Simple.Ecommerce.Contracts.PaymentInformationContracts
{
    public record PaymentInformationRequest
    (
        PaymentMethod PaymentMethod,
        string? PaymentName,
        string? PaymentKey,
        string? ExpirationMonth,
        string? ExpirationYear
    );
}

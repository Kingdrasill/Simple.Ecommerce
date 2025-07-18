using Simple.Ecommerce.Contracts.PaymentInformationContracts;

namespace Simple.Ecommerce.Contracts.UserPaymentContracts
{
    public record UserPaymentRequest
    (
        int UserId,
        PaymentInformationRequest PaymentInformation
    );
}

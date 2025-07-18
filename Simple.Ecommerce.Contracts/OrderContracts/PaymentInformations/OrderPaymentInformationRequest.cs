using Simple.Ecommerce.Contracts.PaymentInformationContracts;

namespace Simple.Ecommerce.Contracts.OrderContracts.PaymentInformations
{
    public record OrderPaymentInformationRequest
    (
        int OrderId,
        PaymentInformationRequest? PaymentInformation = null
    );
}

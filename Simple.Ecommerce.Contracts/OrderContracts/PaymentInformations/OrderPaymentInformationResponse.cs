using Simple.Ecommerce.Contracts.PaymentInformationContracts;

namespace Simple.Ecommerce.Contracts.OrderContracts.PaymentInformations
{
    public record OrderPaymentInformationResponse
    (
        int OrderId,
        PaymentInformationOrderResponse? PaymentInformation = null
    );
}

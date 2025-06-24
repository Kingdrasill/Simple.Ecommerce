using Simple.Ecommerce.Contracts.CardInformationContracts;
using Simple.Ecommerce.Domain.Enums.PaymentMethod;

namespace Simple.Ecommerce.Contracts.OrderContracts
{
    public record OrderPaymentMethodRequest
    (
        int OrderId,
        PaymentMethod PaymentMethod,
        CardInformationRequest? CardInformation = null
    );
}

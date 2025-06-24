using Simple.Ecommerce.Contracts.CardInformationContracts;
using Simple.Ecommerce.Domain.Enums.PaymentMethod;

namespace Simple.Ecommerce.Contracts.OrderContracts
{
    public record OrderPaymentMethodResponse
    (
        int OrderId,
        PaymentMethod? PaymentMethod,
        OrderCardInformationResponse? CardInformation = null
    );
}

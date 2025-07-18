using Simple.Ecommerce.Contracts.PaymentInformationContracts;

namespace Simple.Ecommerce.Contracts.UserPaymentContracts
{
    public record UserPaymentsResponse
    (
        int UserId,
        List<PaymentInformationUserResponse> PaymentInformations
    );
}

using Simple.Ecommerce.Contracts.AddressContracts;
using Simple.Ecommerce.Contracts.PaymentInformationContracts;
using Simple.Ecommerce.Domain.Enums.OrderType;

namespace Simple.Ecommerce.Contracts.OrderContracts.CompleteDTO
{
    public record OrderCompleteRequest
    (
        int UserId,
        OrderType OrderType,
        AddressRequest Address,
        List<OrderItemCompleteRequest> OrderItems,
        PaymentInformationRequest? PaymentInformation = null,
        string? CouponCode = null,
        int? DiscountId = null,
        int Id = 0
    );
}

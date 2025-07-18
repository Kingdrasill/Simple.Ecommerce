using Simple.Ecommerce.Contracts.OrderContracts.PaymentInformations;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Commands.OrderCommands
{
    public interface IChangePaymentInformationOrderCommand
    {
        Task<Result<OrderPaymentInformationResponse>> Execute(OrderPaymentInformationRequest request);
    }
}

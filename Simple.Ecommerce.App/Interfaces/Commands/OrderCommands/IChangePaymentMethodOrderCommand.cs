using Simple.Ecommerce.Contracts.OrderContracts;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Commands.OrderCommands
{
    public interface IChangePaymentMethodOrderCommand
    {
        Task<Result<OrderPaymentMethodResponse>> Execute(OrderPaymentMethodRequest request);
    }
}

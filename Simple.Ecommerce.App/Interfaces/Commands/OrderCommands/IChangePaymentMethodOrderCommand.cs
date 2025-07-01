using Simple.Ecommerce.Contracts.OrderContracts;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Commands.OrderCommands
{
    public interface IChangePaymentMethodOrderCommand
    {
        Task<Result<OrderPaymentMethodResponse>> Execute(OrderPaymentMethodRequest request);
    }
}

using Simple.Ecommerce.Contracts.OrderItemContracts;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Commands.OrderItemCommands
{
    public interface IChangeDiscountOrderItemCommand
    {
        Task<Result<bool>> Execute(OrderItemDiscountRequest request);
    }
}

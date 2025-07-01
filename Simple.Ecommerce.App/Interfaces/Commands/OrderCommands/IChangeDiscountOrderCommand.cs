using Simple.Ecommerce.Contracts.OrderContracts;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.Interfaces.Commands.OrderCommands
{
    public interface IChangeDiscountOrderCommand
    {
        Task<Result<bool>> Execute(OrderDiscountRequest request);
    }
}

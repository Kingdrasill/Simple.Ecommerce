using Simple.Ecommerce.Contracts.OrderContracts;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Commands.OrderCommands
{
    public interface IChangeDiscountOrderCommand
    {
        Task<Result<bool>> Execute(OrderDiscountRequest request);
    }
}

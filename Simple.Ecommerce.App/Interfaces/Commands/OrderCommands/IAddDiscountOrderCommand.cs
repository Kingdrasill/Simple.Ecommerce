using Simple.Ecommerce.Contracts.OrderDiscountContracts;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Commands.OrderCommands
{
    public interface IAddDiscountOrderCommand
    {
        Task<Result<OrderDiscountResponse>> Execute(OrderDiscountRequest request);
    }
}

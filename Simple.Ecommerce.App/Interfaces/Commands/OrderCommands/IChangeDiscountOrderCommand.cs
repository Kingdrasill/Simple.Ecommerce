using Simple.Ecommerce.Contracts.OrderContracts.Discounts;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Commands.OrderCommands
{
    public interface IChangeDiscountOrderCommand
    {
        Task<Result<bool>> Execute(OrderDiscountRequest request);
    }
}

using Simple.Ecommerce.App.Interfaces.Commands.OrderItemCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.UseCases.OrderItemCases.Commands
{
    public class RemoveAllItemsOrderItemCommand : IRemoveAllItemsOrderItemCommand
    {
        private readonly IOrderItemRepository _repository;

        public RemoveAllItemsOrderItemCommand(
            IOrderItemRepository repository
        )
        {
            _repository = repository;
        }

        public async Task<Result<bool>> Execute(int orderId)
        {
            var getOrderItems = await _repository.GetByOrderId(orderId);
            if (getOrderItems.IsFailure)
            {
                return Result<bool>.Failure(getOrderItems.Errors!);
            }

            foreach (var orderItem in getOrderItems.GetValue())
            {
                var deleteResult = await _repository.Delete(orderItem.Id);
                if (deleteResult.IsFailure)
                {
                    return Result<bool>.Failure(deleteResult.Errors!);
                }
            }

            return Result<bool>.Success(true);
        }
    }
}

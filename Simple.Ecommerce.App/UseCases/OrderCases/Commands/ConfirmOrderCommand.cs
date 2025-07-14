using Simple.Ecommerce.App.Interfaces.Commands.OrderCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Services.OrderProcessing.Handlers;
using Simple.Ecommerce.Contracts.OrderContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.OrderProcessing.Commands;

namespace Simple.Ecommerce.App.UseCases.OrderCases.Commands
{
    public class ConfirmOrderCommand : IConfirmOrderCommand
    {
        private readonly ProcessConfirmedOrderCommandHandler _processConfirmedOrderCommandHandler;
        private readonly IOrderRepository _repository;

        public ConfirmOrderCommand(
            ProcessConfirmedOrderCommandHandler processConfirmedOrderCommandHandler,
            IOrderRepository repository
        )
        {
            _processConfirmedOrderCommandHandler = processConfirmedOrderCommandHandler;
            _repository = repository;
        }

        public async Task<Result<OrderCompleteDTO>> Execute(int id)
        {
            var processCommand = new ProcessConfirmedOrderCommand(id);
            var processedResult = await _processConfirmedOrderCommandHandler.Handle(processCommand);
            if (processedResult.IsFailure)
            {
                return Result<OrderCompleteDTO>.Failure(processedResult.Errors!);
            }

            var getCompleteOrder = await _repository.GetCompleteOrder(id);
            if (getCompleteOrder.IsFailure)
            {
                return Result<OrderCompleteDTO>.Failure(getCompleteOrder.Errors!);
            }

            return Result<OrderCompleteDTO>.Success(getCompleteOrder.GetValue());
        }
    }
}

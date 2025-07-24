using Simple.Ecommerce.App.Interfaces.Commands.OrderCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Services.OrderProcessing.Handlers;
using Simple.Ecommerce.Contracts.AddressContracts;
using Simple.Ecommerce.Contracts.OrderContracts;
using Simple.Ecommerce.Contracts.OrderContracts.CompleteDTO;
using Simple.Ecommerce.Contracts.PaymentInformationContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.OrderProcessing.Commands;
using System.Collections.Generic;

namespace Simple.Ecommerce.App.UseCases.OrderCases.Commands
{
    public class RevertProcessedOrderCommand : IRevertProcessedOrderCommand
    {
        private readonly RevertOrderCommandHandler _revertOrderCommandHandler;
        private readonly IOrderRepository _repository;

        public RevertProcessedOrderCommand(
            RevertOrderCommandHandler revertOrderCommandHandler, 
            IOrderRepository repository
        )
        {
            _revertOrderCommandHandler = revertOrderCommandHandler;
            _repository = repository;
        }

        public async Task<Result<OrderResponse>> Execute(int id)
        {
            var revertCommand = new RevertOrderCommand(id);
            var revertedResult = await _revertOrderCommandHandler.Handle(revertCommand);
            if (revertedResult.IsFailure)
            {
                return Result<OrderResponse>.Failure(revertedResult.Errors!);
            }

            var getOrder = await _repository.Get(id);
            if (getOrder.IsFailure)
            {
                List<Error> errors = new List<Error> { new("RevertProcessedOrderCommand.ProcessedError", "O pedido foi revertido mas falhou na hora de buscar os dados do pedido.") };
                errors.AddRange(getOrder.Errors!);
                return Result<OrderResponse>.Failure(errors);
            }
            var order = getOrder.GetValue();

            var response = new OrderResponse(
                order.Id,
                order.UserId,
                order.OrderType,
                new OrderAddressResponse(
                    order.Address.Number,
                    order.Address.Street,
                    order.Address.Neighbourhood,
                    order.Address.City,
                    order.Address.Country,
                    order.Address.Complement,
                    order.Address.CEP
                ),
                order.PaymentInformation is null 
                    ? null
                    : new PaymentInformationOrderResponse(
                        order.PaymentInformation.PaymentMethod,
                        order.PaymentInformation.PaymentName,
                        order.PaymentInformation.PaymentKey,
                        order.PaymentInformation.ExpirationMonth,
                        order.PaymentInformation.ExpirationYear,
                        order.PaymentInformation.CardFlag,
                        order.PaymentInformation.Last4Digits
                    ),
                order.TotalPrice,
                order.OrderDate,
                order.Confirmation,
                order.Status,
                order.DiscountId
            );

            return Result<OrderResponse>.Success(response);
        }
    }
}

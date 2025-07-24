using MongoDB.Bson;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.OrderProcessing;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.OrderProcessing.Events.OrderDiscountEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Models;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Handlers.RevertingHandlers.OrderDiscountHandler
{
    public class ReverOrderDiscountHandler : IOrderRevertingHandler
    {
        public string EventType => nameof(OrderDiscountAppliedEvent);
        private readonly IDiscountRepository _discountRepository;

        public ReverOrderDiscountHandler(
            IDiscountRepository discountRepository
        ) : base() 
        { 
            _discountRepository = discountRepository;
        }

        public async Task<Result<bool>> Revert(OrderInProcess order, BsonDocument eventData)
        {
            var discountId = eventData["DiscountId"].AsInt32;
            var discountName = eventData["DiscountName"].AsString;
            var discountType = (DiscountType)eventData["DiscountType"].AsInt32;
            var amountDiscounted = eventData["AmountDiscounted"].AsDecimal;

            var getDiscount = await _discountRepository.Get(discountId);
            if (getDiscount.IsFailure)
            {
                Console.WriteLine($"\t[ReverOrderDiscountHandler] Os dados do desconto '{discountName}' aplicado ao pedido {order.Id} não foram encontrados.");
                return Result<bool>.Failure(new List<Error>{ new("ReverOrderDiscountHandler.NotFound", $"Os dados do desconto '{discountName}' aplicado ao pedido {order.Id} não foram encontrados.") });
            }
            var discount = getDiscount.GetValue();

            var publishEvent = order.RevertOrderDiscount(discountId, discountName, discountType, amountDiscounted);
            Console.WriteLine($"\t[ReverOrderDiscountHandler] O desconto '{discountName}' foi removido do pedido {order.Id}. Valor Revertido: {publishEvent.AmountReverted:C}. Novo Total: {publishEvent.CurrentTotal:C}");

            order.AddUnappliedDiscount(new OrderDiscountInProcess(
                discountId,
                order.Id,
                discountName,
                discountType,
                discount.DiscountScope,
                discount.DiscountValueType,
                discount.Value,
                discount.ValidFrom,
                discount.ValidTo,
                discount.IsActive
            ));

            return Result<bool>.Success(true);
        }
    }
}

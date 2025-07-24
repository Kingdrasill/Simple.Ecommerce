using MongoDB.Bson;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.OrderProcessing;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ItemSimpleEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Models;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Handlers.RevertingHandlers.ItemsSimpleHandler
{
    public class RevertSimpleItemDiscountHandler : IOrderRevertingHandler
    {
        public string EventType => nameof(SimpleItemDiscountAppliedEvent);
        private readonly IDiscountRepository _discountRepository;

        public RevertSimpleItemDiscountHandler(
            IDiscountRepository discountRepository
        ) : base()
        {
            _discountRepository = discountRepository;
        }

        public async Task<Result<bool>> Revert(OrderInProcess order, BsonDocument eventData)
        {
            var orderItemId = eventData["OrderItemId"].AsInt32;
            var discountId = eventData["DiscountId"].AsInt32;
            var discountName = eventData["DiscountName"].AsString;
            var discountType = (DiscountType)eventData["DiscountType"].AsInt32;
            var amountDiscountedPrice = eventData["AmountDiscountedPrice"].AsDecimal;
            var amountDiscountedTotal = eventData["AmountDiscountedTotal"].AsDecimal;

            var item = order.Items.FirstOrDefault(item => item.Id == orderItemId);
            if (item is null)
            {
                Console.WriteLine($"\t[RevertSimpleItemsDiscountHandler] O produto do pedido {orderItemId} no evento não foi encontrado no pedido {order.Id}.");
                return Result<bool>.Failure(new List<Error>{ new("RevertSimpleItemsDiscountHandler.NotFound", $"O produto do pedido {orderItemId} no evento não foi encontrado no pedido {order.Id}.") });
            }

            var getDiscount = await _discountRepository.Get(discountId);
            if (getDiscount.IsFailure)
            {
                Console.WriteLine($"\t[RevertSimpleItemsDiscountHandler] Os dados do desconto '{discountName}' aplicado ao produto {item.ProductName} não foram encontrados.");
                return Result<bool>.Failure(new List<Error> { new("RevertSimpleItemsDiscountHandler.NotFound", $"Os dados do desconto '{discountName}' aplicado ao produto {item.ProductName} não foram encontrados.") });
            }
            var discount = getDiscount.GetValue();

            var publishEvent = order.RevertSimpleItemDiscount(orderItemId, discountId, discountName, discountType, amountDiscountedPrice, amountDiscountedTotal);
            Console.WriteLine($"\t[RevertSimpleItemsDiscountHandler] O desconto {discountName} foi removido do produto {item.ProductName}. Valor Revertido: {publishEvent.AmountRevertedTotal:C}. Novo Total: {publishEvent.CurrentTotal:C}.");

            order.AddUnappliedDiscount(new OrderDiscountInProcess(
                discountId,
                orderItemId,
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

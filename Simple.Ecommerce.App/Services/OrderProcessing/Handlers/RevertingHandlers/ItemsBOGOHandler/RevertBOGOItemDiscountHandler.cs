using MongoDB.Bson;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.OrderProcessing;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ItemBOGOEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Models;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Handlers.RevertingHandlers.ItemsBOGOHandler
{
    public class RevertBOGOItemDiscountHandler : IOrderRevertingHandler
    {
        public string EventType => nameof(BOGOItemDiscountAppliedEvent);
        private readonly IDiscountRepository _discountRepository;

        public RevertBOGOItemDiscountHandler(
            IDiscountRepository discountRepository
        ) : base()
        {
            _discountRepository = discountRepository;
        }

        public async Task<Result<bool>> Revert(OrderInProcess order, BsonDocument eventData)
        {
            var orderItemId = eventData["OriginalOrderItemId"].AsInt32;
            var discountId = eventData["DiscountId"].AsInt32;
            var discountName = eventData["DiscountName"].AsString;
            var discountType = (DiscountType)eventData["DiscountType"].AsInt32;
            var amountDiscounted = eventData["AmountDiscounted"].AsDecimal;

            var item = order.Items.FirstOrDefault(item => item.Id == orderItemId);
            if (item is null)
            {
                Console.WriteLine($"\t[RevertBOGOItemsDiscountHandler] O produto do pedido {orderItemId} no evento não foi encontrado no pedido {order.Id}.");
                return Result<bool>.Failure(new List<Error> { new("RevertTieredItemDiscountHandler.NotFound", $"O produto do pedido {orderItemId} no evento não foi encontrado no pedido {order.Id}.") });
            }

            var getDiscount = await _discountRepository.Get(discountId);
            if (getDiscount.IsFailure)
            {
                Console.WriteLine($"\t[RevertBOGOItemsDiscountHandler] Os dados do desconto '{discountName}' aplicado ao produto {item.ProductName} não foram encontrados.");
                return Result<bool>.Failure(new List<Error> { new("RevertTieredItemDiscountHandler.NotFound", $"Os dados do desconto '{discountName}' aplicado ao produto {item.ProductName} não foram encontrados.") });
            }
            var discount = getDiscount.GetValue();

            var publishEvent = order.RevertBOGOItemDiscount(orderItemId, discountId, discountName, discountType, amountDiscounted);
            Console.WriteLine($"\t[RevertBOGOItemsDiscountHandler] O desconto {discountName} foi removido do produto {item.ProductName}. Valor Revertido: {publishEvent.AmountReverted:C}. Novo Total: {publishEvent.CurrentTotal:C}.");

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

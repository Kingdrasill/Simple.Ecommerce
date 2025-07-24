using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.OrderProcessing;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ItemBundleEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Models;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Handlers.RevertingHandlers.ItemsBundleHandler
{
    public class RevertBundleItemDiscountHandler : IOrderRevertingHandler
    {
        public string EventType => nameof(BundleDiscountAppliedEvent);
        private readonly IDiscountRepository _discountRepository;

        public RevertBundleItemDiscountHandler(
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
            var bundleId = eventData["BundleId"].AsGuid;
            var bsonArray = eventData["BundleItems"].AsBsonArray;
            var bundleItems = bsonArray
                .Select(b => BsonSerializer.Deserialize<BundleItemEntry>(b.AsBsonDocument))
                .ToList();
            var amountDiscountedTotal = eventData["AmountDiscountedTotal"].AsDecimal;

            List<BundleItemRevertDetail> bItems = new();
            foreach (var bItem in bundleItems)
            {
                var item = order.Items.FirstOrDefault(i => i.Id == bItem.OriginalOrderItemId);
                if (item is null)
                {
                    Console.WriteLine($"\t[RevertBundleItemDiscountHandler] O produto do pedido {bItem.OriginalOrderItemId} no evento não foi encontrado no pedido {order.Id}.");
                    return Result<bool>.Failure(new List<Error>{ new("RevertBundleItemDiscountHandler.NotFound", $"O produto do pedido {bItem.OriginalOrderItemId} no evento não foi encontrado no pedido {order.Id}.") });
                }

                bItems.Add(new BundleItemRevertDetail(
                    bItem.OriginalOrderItemId,
                    bItem.ProductId,
                    bItem.Quantity,
                    bItem.AmountDiscountedPrice
                ));
            }

            var getDiscount = await _discountRepository.Get(discountId);
            if (getDiscount.IsFailure)
            {
                Console.WriteLine($"\t[RevertBundleItemDiscountHandler] Os dados do desconto '{discountName}' não foram encontrados.");
                return Result<bool>.Failure(new List<Error> { new("RevertTieredItemDiscountHandler.NotFound", $"Os dados do desconto '{discountName}' não foram encontrados.") });
            }
            var discount = getDiscount.GetValue();

            var publishEvent = order.RevertBundleItemDiscount(bundleId, bItems, discountId, discountName, discountType, amountDiscountedTotal);
            Console.WriteLine($"\t[RevertBundleItemDiscountHandler] O desconto {discountName} foi removido do pedido {order.Id}, os produtos do pacote foram revertidos aos produtos originiais. Valor Revertido: {publishEvent.AmountRevertedTotal:C}. Novo Total {publishEvent.CurrentTotal:C}.");

            foreach (var bItem in bundleItems)
            {
                order.AddUnappliedDiscount(new OrderDiscountInProcess(
                    discountId,
                    bItem.OriginalOrderItemId,
                    discountName,
                    discountType,
                    discount.DiscountScope,
                    discount.DiscountValueType,
                    discount.Value,
                    discount.ValidFrom,
                    discount.ValidTo,
                    discount.IsActive
                ));
            }

            return Result<bool>.Success(true);
        }
    }
}

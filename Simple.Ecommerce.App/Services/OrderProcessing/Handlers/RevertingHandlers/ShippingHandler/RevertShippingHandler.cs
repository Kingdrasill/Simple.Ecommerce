using MongoDB.Bson;
using Simple.Ecommerce.App.Interfaces.Services.OrderProcessing;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Enums.OrderType;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ShippingEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Models;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Handlers.RevertingHandlers.ShippingHandler
{
    public class RevertShippingHandler : IOrderRevertingHandler
    {
        public string EventType => nameof(ShippingFeeAppliedEvent);

        public RevertShippingHandler() : base() { }

        public async Task<Result<bool>> Revert(OrderInProcess order, BsonDocument eventData)
        {
            if (order.OrderType == OrderType.Delivery)
            {
                var shippingFee = eventData["ShippingFee"].AsDecimal;
                if (order.ShippingFee - shippingFee != 0)
                {
                    Console.WriteLine($"[RevertShippingHandler] O valor de entrega no evento({shippingFee:C}) não zera o valor de entrega no pedido {order.Id}({order.ShippingFee:C}).");
                    return Result<bool>.Failure(new List<Error> { new("RevertTaxHandler.TaxAmount", $"O valor de entrega no evento({shippingFee:C}) não zera o valor de entrega no pedido {order.Id}({order.ShippingFee:C}).") });
                }

                order.RevertShippingFee(shippingFee);
                Console.WriteLine($"\t[RevertShippingHandler] O valor da entrega foi removido do pedido {order.Id}. Valor Revertido: {shippingFee:C}. Novo Total {order.CurrentTotalPrice:C}");
            }

            return Result<bool>.Success(true);
        }
    }
}

using MongoDB.Bson;
using Simple.Ecommerce.App.Interfaces.Services.OrderProcessing;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.OrderProcessing.Events.TaxEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Models;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Handlers.RevertingHandlers.TaxHandler
{
    public class RevertTaxHandler : IOrderRevertingHandler
    {
        public string EventType => nameof(TaxAppliedEvent);

        public RevertTaxHandler () : base() { }

        public async Task<Result<bool>> Revert(OrderInProcess order, BsonDocument eventData)
        {
            var taxAmount = eventData["TaxAmount"].AsDecimal;
            if (order.TaxAmount - taxAmount != 0)
            {
                Console.WriteLine($"[RevertTaxHandler] O valor taxado no evento({taxAmount:C}) não zera o valor taxado no pedido {order.Id}({order.TaxAmount:C}).");
                return Result<bool>.Failure(new List<Error> { new("RevertTaxHandler.TaxAmount", $"O valor taxado no eventoevento({taxAmount:C}) não zera o valor taxado no pedido {order.Id}({order.TaxAmount:C}).") });
            }

            order.RevertTaxes(taxAmount);
            Console.WriteLine($"\t[RevertTaxHandler] O valor do imposto foi removido do pedido {order.Id}. Imposto Revertido: {taxAmount:C}. Novo Total {order.CurrentTotalPrice:C}");

            return Result<bool>.Success(true);
        }
    }
}

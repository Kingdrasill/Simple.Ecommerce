using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.OrderProcessing;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.OrderProcessing.Events.OrderEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.StockEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Models;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Handlers.RevertingHandlers.StockValidationHandler
{
    public class RevertStockValidationHandler : IOrderRevertingHandler
    {
        public string EventType => nameof(OrderProcessingStartedEvent);
        private readonly IProductRepository _productRepository;

        public RevertStockValidationHandler(
            IProductRepository productRepository
        ) : base()
        {
            _productRepository = productRepository;
        }

        public async Task<Result<bool>> Revert(OrderInProcess order, BsonDocument eventData)
        {
            var bsonArray = eventData["Items"].AsBsonArray;
            var items  = bsonArray
                .Select(b => BsonSerializer.Deserialize<OrderItemEntry>(b.AsBsonDocument))
                .ToList();
            foreach (var item in items)
            {
                var getProduct = await _productRepository.Get(item.ProductId);
                if (getProduct.IsFailure)
                {
                    Console.WriteLine($"\t[RevertStockValidationHandler] O produto {item.ProductName} não foi encontrado.");
                    return Result<bool>.Failure(new List<Error>{ new("RevertStockValidationHandler.NotFound", $"O produto {item.ProductName} não foi encontrado.") });
                }
                var product = getProduct.GetValue();
                var orderProduct = order.Items.FirstOrDefault(i => i.Id == item.OrderItemId && i.ProductId == item.ProductId);

                if (orderProduct is null)
                {
                    Console.WriteLine($"\t[RevertStockValidationHandler] O produto {item.ProductName} do evento não foi encontrado no pedido {order.Id}.");
                    return Result<bool>.Failure(new List<Error> { new("RevertStockValidationHandler.NotFound", $"O produto {item.ProductName} do evento não foi encontrado no pedido {order.Id}.") });
                }
                else if (orderProduct.Quantity - item.Quantity != 0)
                {
                    Console.WriteLine($"\t[RevertStockValidationHandler] A quantidade produto {item.ProductName} no evento({item.Quantity}) é diferente da quantidade no pedido {order.Id}({orderProduct.Quantity}).");
                    return Result<bool>.Failure(new List<Error> { new("RevertStockValidationHandler.Quantity", $"O produto {item.ProductName} do evento não foi encontrado no pedido {order.Id}.") });
                }

                product.ChangeStock(item.Quantity);
                var updateProduct = await _productRepository.Update(product, true);
                if (updateProduct.IsFailure)
                {
                    Console.WriteLine($"\t[RevertStockValidationHandler] Falha ao atualizar a quantidade do estoque do produto {item.ProductName}.");
                    return Result<bool>.Failure(updateProduct.Errors!);
                }

                Console.WriteLine($"\t[RevertStockValidationHandler] Desfazendo a reseva de {item.Quantity} do produto {item.ProductName} (ID: {item.ProductId}) para o pedido {order.Id}.");
                var stockRevertEvent = new StockReleasedEvent(
                    order.Id,
                    item.ProductId,
                    item.Quantity
                );
                order.AddEvent(stockRevertEvent);
            }

            return Result<bool>.Success(true);
        }
    }
}

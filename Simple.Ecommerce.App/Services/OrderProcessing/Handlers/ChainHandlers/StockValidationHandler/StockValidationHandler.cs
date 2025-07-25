using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Exceptions.ResultException;
using Simple.Ecommerce.Domain.OrderProcessing.Events.StockEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Models;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Handlers.ChainHandlers.StockValidationHandler
{
    public class StockValidationHandler : BaseOrderProcessingHandler
    {
        private readonly IProductRepository _productRepository;

        public StockValidationHandler(
            IProductRepository productRepository
        ) : base() 
        { 
            _productRepository = productRepository;
        }

        public override async Task Handle(OrderInProcess orderInProcess, bool skipDiscounts = false)
        {
            Console.WriteLine("\t[StockValidationHandler] Validando estoque e reservando itens...");
            foreach (var item in orderInProcess.Items)
            {
                var getProduct = await _productRepository.Get(item.ProductId);
                if (getProduct.IsFailure)
                {
                    throw new ResultException(new Error("StockValidationHandler.NotFound", $"O produto {item.ProductName} não foi encontrado no estoque!"));
                }

                var product = getProduct.GetValue();
                if (product.Stock < item.Quantity)
                {
                    throw new ResultException(new Error("StockValidationHandler.Missing", $"O produto {item.ProductName} só possui em estoque {product.Stock} e para esse pedido é necessário {item.Quantity}!"));
                }

                product.ChangeStock(-item.Quantity);
                if (product.Validate() is { IsFailure: true } result)
                {
                    throw new ResultException(result.Errors!);
                }

                var updateProduct = await _productRepository.Update(product, true);
                if (updateProduct.IsFailure)
                {
                    throw new ResultException(new Error("StockValidationHandler.StockChange", $"Não foi possível alterar o estoque do produto {item.ProductName}!"));
                }

                Console.WriteLine($"\t[StockValidationHandler] Reservando {item.Quantity} de {item.ProductName} (Produto ID: {item.Id})");
                var stockReservationEvent = new StockReservedEvent(
                    orderInProcess.Id,
                    item.ProductId,
                    item.Quantity
                );
                orderInProcess.AddEvent(stockReservationEvent);
            } 

            // Passando para o Próximo Handler
            await base.Handle(orderInProcess, skipDiscounts);
        }
    }
}

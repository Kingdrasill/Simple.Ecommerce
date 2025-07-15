using Simple.Ecommerce.App.Interfaces.Services.OrderProcessing;
using Simple.Ecommerce.App.Services.OrderProcessing.Handlers.ChainHandlers.DiscountsValidationHandler;
using Simple.Ecommerce.App.Services.OrderProcessing.Handlers.ChainHandlers.ItemsBOGOHandler;
using Simple.Ecommerce.App.Services.OrderProcessing.Handlers.ChainHandlers.ItemsBundleHandler;
using Simple.Ecommerce.App.Services.OrderProcessing.Handlers.ChainHandlers.ItemsSimpleHandler;
using Simple.Ecommerce.App.Services.OrderProcessing.Handlers.ChainHandlers.ItemsTieredHandler;
using Simple.Ecommerce.App.Services.OrderProcessing.Handlers.ChainHandlers.OrderDiscountHandler;
using Simple.Ecommerce.App.Services.OrderProcessing.Handlers.ChainHandlers.ShippingHandler;
using Simple.Ecommerce.App.Services.OrderProcessing.Handlers.ChainHandlers.StockValidationHandler;
using Simple.Ecommerce.App.Services.OrderProcessing.Handlers.ChainHandlers.TaxHandler;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Exceptions.ResultException;
using Simple.Ecommerce.Domain.OrderProcessing.Models;

namespace Simple.Ecommerce.App.Services.OrderProcessing.ChainOfResponsibility
{
    public class OrderProcessingChain : IOrderProcessingChain
    {
        private readonly IOrderProcessingHandler _firstHandler;

        public OrderProcessingChain(
            DiscountsValidationHandler discountsValidationHandler,
            BOGOItemsDiscountHandler bogoItemsDiscountHandler,
            BundleItemsDiscountHandler bundleItemsDiscountHandler,
            SimpleItemsDiscountHandler simpleItemsDiscountHandler,
            TieredItemsDiscountHandler tieredItemsDiscountHandler,
            OrderDiscountHandler orderDiscountHandler,
            ShippingHandler shippingHandler,
            StockValidationHandler stockValidationHandler,
            TaxHandler taxHandler
        )
        {
            stockValidationHandler
                .SetNext(shippingHandler);
            shippingHandler
                .SetNext(discountsValidationHandler);
            discountsValidationHandler
                .SetNext(simpleItemsDiscountHandler);
            simpleItemsDiscountHandler
                .SetNext(tieredItemsDiscountHandler);
            tieredItemsDiscountHandler
                .SetNext(bogoItemsDiscountHandler);
            bogoItemsDiscountHandler
                .SetNext(bundleItemsDiscountHandler);
            bundleItemsDiscountHandler
                .SetNext(orderDiscountHandler);
            orderDiscountHandler
                .SetNext(taxHandler);
            // Add a payment method handler

            _firstHandler = stockValidationHandler;
        }

        public async Task<Result<bool>> Process(OrderInProcess order, bool skipDiscounts = false)
        {
            try
            {
                await _firstHandler.Handle(order, skipDiscounts);
                return Result<bool>.Success(true);
            }
            catch (ResultException rex)
            {
                var errors = new List<Error>();
                Console.WriteLine($"\t[OrderProcessingChain] Erro ao processar o pedido {order.Id}.");
                errors.AddRange(rex.Errors!);
                return Result<bool>.Failure(errors);
            }
            catch (ArgumentException aex)
            {
                var errors = new List<Error>();
                Console.WriteLine($"\t[OrderProcessingChain] Erro ao processar o pedido {order.Id}: {aex.Message}.");
                errors.Add(new("OrderProcessingChain.ValueError", aex.Message));
                return Result<bool>.Failure(errors);
            }
            catch (InvalidOperationException ioex)
            {
                var errors = new List<Error>();
                Console.WriteLine($"\t[OrderProcessingChain] Erro ao processar o pedido {order.Id}: {ioex.Message}.");
                errors.Add(new("OrderProcessingChain.InvalidOperationError", ioex.Message));
                return Result<bool>.Failure(errors);
            }
            catch (Exception ex)
            {
                var errors = new List<Error>();
                Console.WriteLine($"\t[OrderProcessingChain] Erro ao processar o pedido {order.Id}: {ex.Message}.");
                errors.Add(new("OrderProcessingChain.UnknownError", ex.Message));
                return Result<bool>.Failure(errors);
            }
        }
    }
}

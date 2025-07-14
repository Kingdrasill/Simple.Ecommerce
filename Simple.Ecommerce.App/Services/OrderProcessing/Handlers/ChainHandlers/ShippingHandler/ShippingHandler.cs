using Simple.Ecommerce.Domain.Enums.OrderType;
using Simple.Ecommerce.Domain.OrderProcessing.Models;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Handlers.ChainHandlers.ShippingHandler
{
    public class ShippingHandler : BaseOrderProcessingHandler
    {
        public ShippingHandler() : base() { }

        public override async Task Handle(OrderInProcess orderInProcess, bool skipDiscounts = false)
        {
            // Implementar Lógica de Frete Mais Real
            if (orderInProcess.OrderType == OrderType.Delivery)
            {
                decimal shippingFee = 15.00m;
                orderInProcess.ApplyShippingFee(shippingFee);

                // Publicando o Evento
                Console.WriteLine($"\t[ShippingHandler] Frete calculado: {shippingFee}. Novo Total {orderInProcess.CurrentTotalPrice:C}");
            }

            // Passando para o Próximo Handler
            await base.Handle(orderInProcess, skipDiscounts);
        }
    }
}

using Simple.Ecommerce.Domain.OrderProcessing.Models;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Handlers.ChainHandlers.TaxHandler
{
    public class TaxHandler : BaseOrderProcessingHandler
    {
        public TaxHandler() : base() { }

        public override async Task Handle(OrderInProcess orderInProcess, bool skipDiscounts = false)
        {
            // Implementar uma Lógica de Imposto mais Real
            decimal taxAmount = orderInProcess.CurrentTotalPrice * 0.1m;
            orderInProcess.ApplyTaxes(taxAmount);

            // Publicando o Evento
            Console.WriteLine($"\t[TaxHandler] Imposto calculado: {taxAmount}. Novo Total {orderInProcess.CurrentTotalPrice:C}");
            
            // Bassando para o Próximo Handler
            await base.Handle(orderInProcess);
        }
    }
}

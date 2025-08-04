namespace Simple.Ecommerce.Domain.OrderProcessing.Events.TaxEvent
{
    public class TaxRevertedEvent : TaxEvent
    {
        public TaxRevertedEvent(int orderId, decimal taxAmount, decimal currentTotal)
            : base(orderId, taxAmount, currentTotal) { }
    }
}

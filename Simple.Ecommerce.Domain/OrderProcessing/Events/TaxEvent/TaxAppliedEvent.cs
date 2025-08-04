namespace Simple.Ecommerce.Domain.OrderProcessing.Events.TaxEvent
{
    public class TaxAppliedEvent : TaxEvent
    {
        public TaxAppliedEvent(int orderId, decimal taxAmount, decimal currentTotal)
            : base(orderId, taxAmount, currentTotal) { }
    }
}

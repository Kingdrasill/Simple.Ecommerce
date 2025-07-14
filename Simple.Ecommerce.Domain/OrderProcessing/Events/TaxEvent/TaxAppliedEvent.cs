namespace Simple.Ecommerce.Domain.OrderProcessing.Events.TaxEvent
{
    public class TaxAppliedEvent : OrderProcessingEvent
    {
        public decimal TaxAmount { get; private set; }
        public decimal CurrentTotal { get; private set; }

        public TaxAppliedEvent(int orderId, decimal taxAmount, decimal currentTotal)
            : base(orderId)
        {
            TaxAmount = taxAmount;
            CurrentTotal = currentTotal;
        }
    }
}

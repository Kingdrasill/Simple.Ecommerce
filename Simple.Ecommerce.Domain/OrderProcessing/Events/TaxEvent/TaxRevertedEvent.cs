namespace Simple.Ecommerce.Domain.OrderProcessing.Events.TaxEvent
{
    public class TaxRevertedEvent : OrderProcessingEvent
    {
        public decimal TaxAmount { get; private set; }
        public decimal CurrentTotal { get; private set; }

        public TaxRevertedEvent(int orderId, decimal taxAmount, decimal currentTotal)
            : base(orderId)
        {
            TaxAmount = taxAmount;
            CurrentTotal = currentTotal;
        }
    }
}

namespace Simple.Ecommerce.Domain.OrderProcessing.Events.TaxEvent
{
    public class TaxEvent : BaseOrderProcessingEvent
    {
        public decimal TaxAmount { get; private set; }
        public decimal CurrentTotal { get; private set; }

        public TaxEvent(int orderId, decimal taxAmount, decimal currentTotal)
            : base(orderId)
        {
            TaxAmount = taxAmount;
            CurrentTotal = currentTotal;
        }
    }
}

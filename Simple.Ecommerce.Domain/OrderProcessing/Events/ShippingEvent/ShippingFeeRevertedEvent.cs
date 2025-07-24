namespace Simple.Ecommerce.Domain.OrderProcessing.Events.ShippingEvent
{
    public class ShippingFeeRevertedEvent : OrderProcessingEvent
    {
        public decimal ShippingFee { get; private set; }
        public decimal CurrentTotal { get; private set; }

        public ShippingFeeRevertedEvent(int orderId, decimal shippingFee, decimal currentTotal)
            : base(orderId)
        {
            ShippingFee = shippingFee;
            CurrentTotal = currentTotal;
        }
    }
}

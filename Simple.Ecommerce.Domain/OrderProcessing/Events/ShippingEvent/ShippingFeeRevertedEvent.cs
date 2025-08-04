namespace Simple.Ecommerce.Domain.OrderProcessing.Events.ShippingEvent
{
    public class ShippingFeeRevertedEvent : ShippingFeeEvent
    {
        public ShippingFeeRevertedEvent(int orderId, decimal shippingFee, decimal currentTotal)
            : base(orderId, shippingFee, currentTotal) { }
    }
}

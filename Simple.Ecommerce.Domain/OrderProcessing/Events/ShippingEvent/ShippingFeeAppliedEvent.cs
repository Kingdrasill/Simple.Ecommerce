namespace Simple.Ecommerce.Domain.OrderProcessing.Events.ShippingEvent
{
    public class ShippingFeeAppliedEvent : ShippingFeeEvent
    {
        public ShippingFeeAppliedEvent(int orderId,  decimal shippingFee, decimal currentTotal) 
            : base(orderId, shippingFee, currentTotal) { }
    }
}

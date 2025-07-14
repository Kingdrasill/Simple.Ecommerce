namespace Simple.Ecommerce.Domain.OrderProcessing.Events.ShippingEvent
{
    public class ShippingFeeAppliedEvent : OrderProcessingEvent
    {
        public decimal ShippingFee { get; private set; }
        public decimal CurrentTotal { get; private set; }

        public ShippingFeeAppliedEvent(int orderId,  decimal shippingFee, decimal currentTotal) 
            : base(orderId)
        {
            ShippingFee = shippingFee;
            CurrentTotal = currentTotal;
        }
    }
}

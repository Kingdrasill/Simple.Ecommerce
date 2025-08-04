namespace Simple.Ecommerce.Domain.OrderProcessing.Events.ShippingEvent
{
    public class ShippingFeeEvent : BaseOrderProcessingEvent
    {
        public decimal ShippingFee { get; private set; }
        public decimal CurrentTotal { get; private set; }

        public ShippingFeeEvent(int orderId, decimal shippingFee, decimal currentTotal)
            : base(orderId)
        {
            ShippingFee = shippingFee;
            CurrentTotal = currentTotal;
        }
    }
}

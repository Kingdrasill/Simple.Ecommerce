using Simple.Ecommerce.Domain.Enums.OrderLock;
using Simple.Ecommerce.Domain.Enums.OrderType;
using Simple.Ecommerce.Domain.OrderProcessing.Models;
using Simple.Ecommerce.Domain.ValueObjects.AddressObject;
using Simple.Ecommerce.Domain.ValueObjects.PaymentInformationObject;

namespace Simple.Ecommerce.Domain.OrderProcessing.Events.OrderEvent
{
    public class OrderRevertingStartedEvent : OrderProcessingEvent
    {
        public int UserId { get; set; }
        public OrderType OrderType { get; set; }
        public Address Address { get; set; }
        public PaymentInformation? PaymentInformation { get; set; }
        public OrderLock OrderLock { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal FinalTotal { get; set; }
        public decimal AmountDiscounted { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TaxAmount { get; set; }
        public string Status { get; set; }
        public List<OrderRevertItemEntry> Items { get; set; }
        public List<OrderRevertItemEntry> FreeItems { get; set; }
        public List<OrderRevertBundle> Bundles { get; set; }
        public (int, string)? AppliedDiscount { get; set; }

        public OrderRevertingStartedEvent(int orderId, int userId, OrderType orderType, Address address, PaymentInformation? paymentInformation, OrderLock orderLock, DateTime orderDate, decimal finalTotal, decimal amountDiscounted, decimal shippingFee, decimal taxAmount, string status, List<OrderRevertItemEntry> items, List<OrderRevertItemEntry> freeItems, List<OrderRevertBundle> bundles, (int, string)? appliedDiscount)
            : base(orderId)    
        {
            UserId = userId;
            OrderType = orderType;
            Address = address;
            PaymentInformation = paymentInformation;
            OrderLock = orderLock;
            OrderDate = orderDate;
            FinalTotal = finalTotal;
            AmountDiscounted = amountDiscounted;
            ShippingFee = shippingFee;
            TaxAmount = taxAmount;
            Status = status;
            Items = items;
            FreeItems = freeItems;
            Bundles = bundles;
            AppliedDiscount = appliedDiscount;
        }
    }
}

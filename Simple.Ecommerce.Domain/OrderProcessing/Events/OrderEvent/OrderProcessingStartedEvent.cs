using Simple.Ecommerce.Domain.Enums.OrderLock;
using Simple.Ecommerce.Domain.Enums.OrderType;
using Simple.Ecommerce.Domain.ValueObjects.AddressObject;
using Simple.Ecommerce.Domain.ValueObjects.PaymentInformationObject;

namespace Simple.Ecommerce.Domain.OrderProcessing.Events.OrderEvent
{
    public class OrderProcessingStartedEvent : OrderProcessingEvent
    {
        public int UserId { get; private set; }
        public string UserName { get; private set; }
        public OrderType OrderType { get; private set; }
        public Address Address { get; private set; }
        public PaymentInformation? PaymentInformation { get; private set; }
        public OrderLock OrderLock { get; private set; }
        public DateTime OrderDate { get; private set; }
        public string Status { get; private set; }
        public decimal InitialTotal { get; private set; }
        public List<OrderItemEntry> Items { get; private set; }

        public OrderProcessingStartedEvent(int orderId, int userId, string userName, OrderType orderType, Address address,PaymentInformation? paymentInformation, OrderLock orderLock, DateTime orderDate, string Status, decimal initialTotal, List<OrderItemEntry> items)
            : base(orderId)
        {
            UserId = userId;
            UserName = userName;
            OrderType = orderType;
            Address = address;
            PaymentInformation = paymentInformation;
            OrderLock = orderLock;
            OrderDate = orderDate;
            this.Status = Status;
            InitialTotal = initialTotal;
            Items = items ?? new List<OrderItemEntry>();
        }
    }
}

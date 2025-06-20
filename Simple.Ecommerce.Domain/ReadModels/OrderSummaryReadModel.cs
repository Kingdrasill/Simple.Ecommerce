using Simple.Ecommerce.Domain.Enums.OrderType;
using Simple.Ecommerce.Domain.ValueObjects.AddressObject;
using Simple.Ecommerce.Domain.ValueObjects.OrderItemObject;

namespace Simple.Ecommerce.Domain.ReadModels
{
    public class OrderSummaryReadModel : BaseReadModel
    {
        public string OrderId { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public OrderType OrderType { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentMethod { get; set; }
        public Address Address { get; set; }
        public List<OrderItemEntry> Items { get; set; }
    }
}

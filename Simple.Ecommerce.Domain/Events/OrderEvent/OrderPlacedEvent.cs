using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Enums.OrderType;
using Simple.Ecommerce.Domain.Enums.PaymentMethod;
using Simple.Ecommerce.Domain.Objects;
using Simple.Ecommerce.Domain.ValueObjects.AddressObject;

namespace Simple.Ecommerce.Domain.Events.OrderEvent
{
    public class OrderPlacedEvent
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public OrderType OrderType { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public decimal TotalPrice { get; set; }
        public Address Address { get; set; }
        public List<OrderItemEntry> Items { get; set; }

        public OrderPlacedEvent(Order order)
        {
            OrderId = order.Id;
            UserId = order.UserId;
            OrderDate = order.OrderDate!.Value;
            Status = order.Status;
            OrderType = order.OrderType;
            TotalPrice = order.TotalPrice!.Value;
            PaymentMethod = order.PaymentMethod ?? PaymentMethod.Cash;
            Address = order.Address;
            Items = order.OrderItems.Select(ci => new OrderItemEntry()
            {
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,
                UnitPrice = ci.Price
            }).ToList();
        }
    }
}


using Simple.Ecommerce.App.Interfaces.Services.Command;
using Simple.Ecommerce.Domain.Enums.OrderType;
using Simple.Ecommerce.Domain.ValueObjects.AddressObject;
using Simple.Ecommerce.Domain.ValueObjects.OrderItemObject;

namespace Simple.Ecommerce.App.Services.Command
{
    public class PlaceOrderCommand : ICommand
    {
        public int UserId { get; set; }
        public OrderType OrderType { get; set; }
        public string PaymentMethod { get; set; }
        public Address Address { get; set; }
        public List<OrderItemEntry> Items { get; set; }

    }
}

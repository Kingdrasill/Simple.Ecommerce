using Simple.Ecommerce.Domain.Entities.OrderDiscountEnity;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain.Enums.OrderType;
using Simple.Ecommerce.Domain.Events.DeletedEvent;
using Simple.Ecommerce.Domain.Validation.Validators;
using Simple.Ecommerce.Domain.ValueObjects.AddressObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Simple.Ecommerce.Domain.Entities.OrderEntity
{
    public class Order : BaseEntity
    {
        public DateTime OrderDate { get; private set; }
        public int UserId { get; private set; }
        public User User { get; private set; } = null!;
        public decimal TotalPrice { get; private set; }
        public Address Address { get; private set; }
        public OrderType OrderType { get; private set; }
        public bool Confirmation { get; private set; }
        public string Status { get; private set; }
        public string? PaymentMethod { get; private set; }
        [IgnoreDataMember, NotMapped]
        public ICollection<OrderItem> OrderItems { get; private set; }
        [IgnoreDataMember, NotMapped]
        public ICollection<OrderDiscount> OrderDiscounts { get; private set; }

        public Order() 
        {
            OrderItems = new HashSet<OrderItem>();
            OrderDiscounts = new HashSet<OrderDiscount>();
        }

        private Order(int id, DateTime orderDate, int userId, decimal totalPrice, Address address, OrderType orderType, bool confirmation, string status, string? paymentMethod)
        {
            Id = id;
            OrderDate = orderDate;
            UserId = userId;
            TotalPrice = totalPrice;
            Address = address;
            OrderType = orderType;
            Confirmation = confirmation;
            Status = status;
            PaymentMethod = paymentMethod;

            OrderItems = new HashSet<OrderItem>();
            OrderDiscounts = new HashSet<OrderDiscount>();
        }

        public Result<Order> Create(int id, DateTime orderDate, int userId, decimal totalPrice, Address address, OrderType orderType, bool confirmation, string status, string? paymentMethod)
        {
            return new OrderValidator().Validate(new Order(id, orderDate, userId, totalPrice, address, orderType, confirmation, status, paymentMethod));
        }

        public void Confirm()
        {
            Confirmation = true;
            Status = "Confirmado";
        }

        public void Cancel()
        {
            Confirmation = false;
            Status = "Cancelado";
        }

        public override void MarkAsDeleted(bool raiseEvent = true)
        {
            if (Deleted)
                return;

            Deleted = true;

            if (raiseEvent)
                AddDomainEvent(new OrderDeletedEvent(Id));
        }
    }
}

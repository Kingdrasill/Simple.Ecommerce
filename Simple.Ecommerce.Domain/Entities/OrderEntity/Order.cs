using Simple.Ecommerce.Domain.Entities.OrderDiscountEnity;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain.Enums.OrderType;
using Simple.Ecommerce.Domain.Enums.PaymentMethod;
using Simple.Ecommerce.Domain.Events.DeletedEvent;
using Simple.Ecommerce.Domain.Validation.Validators;
using Simple.Ecommerce.Domain.ValueObjects.AddressObject;
using Simple.Ecommerce.Domain.ValueObjects.CardInformationObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Simple.Ecommerce.Domain.Entities.OrderEntity
{
    public class Order : BaseEntity
    {
        public int UserId { get; private set; }
        public User User { get; private set; } = null!;
        public OrderType OrderType { get; private set; }
        public Address Address { get; private set; }
        public PaymentMethod? PaymentMethod { get; private set; }
        public decimal? TotalPrice { get; private set; }
        public DateTime? OrderDate { get; private set; }
        public bool Confirmation { get; private set; }
        public string Status { get; private set; }
        public CardInformation? CardInformation { get; private set; } = null;
        [IgnoreDataMember, NotMapped]
        public ICollection<OrderItem> OrderItems { get; private set; }
        [IgnoreDataMember, NotMapped]
        public ICollection<OrderDiscount> OrderDiscounts { get; private set; }

        public Order() 
        {
            OrderItems = new HashSet<OrderItem>();
            OrderDiscounts = new HashSet<OrderDiscount>();
        }

        private Order(int id, int userId, OrderType orderType, Address address, PaymentMethod? paymentMethod, decimal? totalPrice, DateTime? orderDate, bool confirmation, string status, CardInformation? cardInformation = null)
        {
            Id = id;
            UserId = userId;
            OrderType = orderType;
            Address = address;
            PaymentMethod = paymentMethod;
            TotalPrice = totalPrice;
            OrderDate = orderDate;
            Confirmation = confirmation;
            Status = status;
            CardInformation = cardInformation;

            OrderItems = new HashSet<OrderItem>();
            OrderDiscounts = new HashSet<OrderDiscount>();
        }

        public Result<Order> Create(int id, int userId, OrderType orderType, Address address, PaymentMethod? paymentMethod, decimal? totalPrice, DateTime? orderDate, bool confirmation, string status)
        {
            return new OrderValidator().Validate(new Order(id, userId, orderType, address, paymentMethod, totalPrice, orderDate, confirmation, status));
        }

        public void UpdatePaymentMethod(PaymentMethod? paymentmethod, CardInformation? cardInformation)
        {
            PaymentMethod = paymentmethod;
            CardInformation = cardInformation;
        }

        public void Confirm()
        {
            OrderDate = DateTime.UtcNow;
            Confirmation = true;
            Status = "Confirmed";
        }

        public void Cancel()
        {
            OrderDate = DateTime.UtcNow;
            Confirmation = false;
            Status = "Canceled";
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

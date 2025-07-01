using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain.Enums.OrderType;
using Simple.Ecommerce.Domain.Enums.PaymentMethod;
using Simple.Ecommerce.Domain.Events.DeletedEvent;
using Simple.Ecommerce.Domain.Objects;
using Simple.Ecommerce.Domain.Validation.Validators;
using Simple.Ecommerce.Domain.ValueObjects.AddressObject;
using Simple.Ecommerce.Domain.ValueObjects.CardInformationObject;
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
        public int? DiscountId { get; private set; }
        public Discount? Discount { get; private set; } = null;
        [IgnoreDataMember, NotMapped]
        public ICollection<OrderItem> OrderItems { get; private set; }

        public Order() 
        {
            OrderItems = new HashSet<OrderItem>();
        }

        private Order(int id, int userId, OrderType orderType, Address address, PaymentMethod? paymentMethod, decimal? totalPrice, DateTime? orderDate, bool confirmation, string status, int? discountId, CardInformation? cardInformation = null)
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
            DiscountId = discountId;
            CardInformation = cardInformation;

            OrderItems = new HashSet<OrderItem>();
        }

        public Result<Order> Create(int id, int userId, OrderType orderType, Address address, PaymentMethod? paymentMethod, decimal? totalPrice, DateTime? orderDate, bool confirmation, string status, int? discountId, CardInformation? cardInformation = null)
        {
            return new OrderValidator().Validate(new Order(id, userId, orderType, address, paymentMethod, totalPrice, orderDate, confirmation, status, discountId, cardInformation));
        }

        public void UpdateDiscountId(int? discountId)
        {
            DiscountId = discountId;
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

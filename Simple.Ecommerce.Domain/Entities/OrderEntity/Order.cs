using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain.EntityDeletionEvents;
using Simple.Ecommerce.Domain.Enums.OrderType;
using Simple.Ecommerce.Domain.Validation.Validators;
using Simple.Ecommerce.Domain.ValueObjects.AddressObject;
using Simple.Ecommerce.Domain.ValueObjects.PaymentInformationObject;
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
        public decimal? TotalPrice { get; private set; }
        public DateTime? OrderDate { get; private set; }
        public bool Confirmation { get; private set; }
        public string Status { get; private set; }
        public int? DiscountId { get; private set; }
        public Discount? Discount { get; private set; } = null;
        public PaymentInformation? PaymentInformation { get; private set; } = null;
        [IgnoreDataMember, NotMapped]
        public ICollection<OrderItem> OrderItems { get; private set; }

        public Order() 
        {
            OrderItems = new HashSet<OrderItem>();
        }

        private Order(int id, int userId, OrderType orderType, Address address, decimal? totalPrice, DateTime? orderDate, bool confirmation, string status, int? discountId, PaymentInformation? paymentInformation = null)
        {
            Id = id;
            UserId = userId;
            OrderType = orderType;
            Address = address;
            TotalPrice = totalPrice;
            OrderDate = orderDate;
            Confirmation = confirmation;
            Status = status;
            DiscountId = discountId;
            PaymentInformation = paymentInformation;

            OrderItems = new HashSet<OrderItem>();
        }

        public Result<Order> Create(int id, int userId, OrderType orderType, Address address, decimal? totalPrice, DateTime? orderDate, bool confirmation, string status, int? discountId, PaymentInformation? paymentInformation = null)
        {
            return new OrderValidator().Validate(new Order(id, userId, orderType, address, totalPrice, orderDate, confirmation, status, discountId, paymentInformation));
        }

        public void UpdateDiscountId(int? discountId)
        {
            DiscountId = discountId;
        }

        public void UpdatePaymentInformation(PaymentInformation? paymentInformation)
        {
            PaymentInformation = paymentInformation;
        }

        public void UpdateStatus(string status, bool? confirmation = null, decimal? newTotalPrice = null)
        {
            OrderDate = DateTime.UtcNow;
            Status = status;
            if (confirmation.HasValue)
            {
                Confirmation = confirmation.Value;
            }
            if (newTotalPrice.HasValue)
            {
                TotalPrice = newTotalPrice;
            }
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

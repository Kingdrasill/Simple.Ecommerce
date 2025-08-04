using Simple.Ecommerce.Domain.Entities.CouponEntity;
using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain.EntityDeletionEvents;
using Simple.Ecommerce.Domain.Enums.OrderLock;
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
        public int? CouponId { get; private set; }
        public Coupon? Coupon { get; private set; } = null;
        public int? DiscountId { get; private set; }
        public Discount? Discount { get; private set; } = null;
        public PaymentInformation? PaymentInformation { get; private set; }
        public OrderLock OrderLock { get; private set; }
        [IgnoreDataMember, NotMapped]
        public ICollection<OrderItem> OrderItems { get; private set; }

        public Order() 
        {
            OrderItems = new HashSet<OrderItem>();
        }

        private Order(int id, int userId, OrderType orderType, Address address, decimal? totalPrice, DateTime? orderDate, bool confirmation, string status, int? couponId, int? discountId, OrderLock orderLock, PaymentInformation? paymentInformation)
        {
            Id = id;
            UserId = userId;
            OrderType = orderType;
            Address = address;
            TotalPrice = totalPrice;
            OrderDate = orderDate;
            Confirmation = confirmation;
            Status = status;
            CouponId = couponId;
            DiscountId = discountId;
            PaymentInformation = paymentInformation;
            OrderLock = orderLock;

            OrderItems = new HashSet<OrderItem>();
        }

        public Result<Order> Create(int id, int userId, OrderType orderType, Address address, decimal? totalPrice, DateTime? orderDate, bool confirmation, string status, int? couponId, int? discountId, OrderLock orderLock, PaymentInformation? paymentInformation)
        {
            return new OrderValidator().Validate(new Order(id, userId, orderType, address, totalPrice, orderDate, confirmation, status, couponId, discountId, orderLock, paymentInformation));
        }

        public Result<Order> Validate()
        {
            return new OrderValidator().Validate(this);
        }

        public void UpdateAddress(Address address) => Address = address;
        public void UpdatePaymentInformation(PaymentInformation? paymentInformation) => PaymentInformation = paymentInformation;

        public void UpdateDiscount(int? couponId, int? discountId)
        {
            CouponId = couponId;
            DiscountId = discountId;
        }
        
        public void UpdateStatus(string status, OrderLock orderLock, bool? confirmation = null, decimal? newTotalPrice = null)
        {
            OrderDate = DateTime.UtcNow;
            Status = status;
            OrderLock = orderLock;
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

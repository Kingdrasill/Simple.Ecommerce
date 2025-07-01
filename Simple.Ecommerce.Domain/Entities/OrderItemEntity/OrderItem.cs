using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Entities.ProductEntity;
using Simple.Ecommerce.Domain.Events.DeletedEvent;
using Simple.Ecommerce.Domain.Objects;
using Simple.Ecommerce.Domain.Validation.Validators;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Simple.Ecommerce.Domain.Entities.OrderItemEntity
{
    public class OrderItem : BaseEntity
    {
        public decimal Price { get; private set; }
        public int Quantity { get; private set; }
        public int ProductId { get; private set; }
        public Product Product { get; private set; } = null!;
        public int OrderId { get; private set; }
        public Order Order { get; private set; } = null!;
        public int? DiscountId { get; private set; }
        public Discount? Discount { get; private set; } = null;
        [IgnoreDataMember, NotMapped]
        public decimal Total => Price * Quantity;

        public OrderItem() { }

        private OrderItem(int id, decimal price, int quantity, int productId, int orderId, int? discountId)
        {
            Id = id;
            Price = price;
            Quantity = quantity;
            ProductId = productId;
            OrderId = orderId;
            DiscountId = discountId;
        }

        public Result<OrderItem> Create(int id, decimal price, int quantity, int productId, int orderId, int? discountId)
        {
            return new OrderItemValidator().Validate(new OrderItem(id, price, quantity, productId, orderId, discountId));
        }

        public Result<OrderItem> Validate()
        {
            return new OrderItemValidator().Validate(this);
        }

        public void Update(int quantity, decimal price, int? discountId, bool overrideQuantity)
        {
            if (overrideQuantity)
            {
                Quantity = quantity;
            }
            else
            {
                Quantity += quantity;
            }
            Price = price;
            DiscountId = discountId;
        }

        public void UpdateDiscountId(int? discountId)
        {
            DiscountId = discountId;
        }

        public override void MarkAsDeleted(bool raiseEvent = true)
        {
            if (Deleted)
                return;

            Deleted = true;

            if (raiseEvent)
                AddDomainEvent(new CartItemDeletedEvent(Id));
        }
    }
}

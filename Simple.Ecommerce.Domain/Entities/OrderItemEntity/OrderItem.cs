using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Entities.ProductEntity;
using Simple.Ecommerce.Domain.Events.DeletedEvent;
using Simple.Ecommerce.Domain.Validation.Validators;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Simple.Ecommerce.Domain.Entities.OrderItemEntity
{
    public class OrderItem : BaseEntity
    {
        public int ProductId { get; set; }
        public Product Product { get; private set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int OrderId { get; private set; }
        public Order Order { get; private set; } = null!;

        [IgnoreDataMember, NotMapped]
        public decimal Total => Price * Quantity;

        public OrderItem() { }

        private OrderItem(int id, int productId, decimal price, int quantity, int orderId)
        {
            Id = id;
            ProductId = productId;
            Price = price;
            Quantity = quantity;
            OrderId = orderId;
        }

        public Result<OrderItem> Create(int id, int productId, decimal price, int quantity, int orderId)
        {
            return new CartItemValidator().Validate(new OrderItem(id, productId, price, quantity, orderId));
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

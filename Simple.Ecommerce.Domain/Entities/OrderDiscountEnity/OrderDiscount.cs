using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Events.DeletedEvent;
using Simple.Ecommerce.Domain.Validation.Validators;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.Domain.Entities.OrderDiscountEnity
{
    public class OrderDiscount : BaseEntity
    {
        public int OrderId { get; private set; }
        public Order Order { get; private set; } = null!;
        public int DiscountId { get; private set; }
        public Discount Discount { get; private set; } = null!;

        public OrderDiscount() { }

        private OrderDiscount(int id, int orderId, int discountId)
        {
            Id = id;
            OrderId = orderId;
            DiscountId = discountId;
        }

        public Result<OrderDiscount> Create(int id, int orderId, int discountId)
        {
            return new OrderDiscountValidator().Validate(new OrderDiscount(id, orderId, discountId));
        }

        public override void MarkAsDeleted(bool raiseEvent = true)
        {
            if (Deleted)
                return;

            Deleted = true;

            if (raiseEvent)
                AddDomainEvent(new OrderDiscountDeletedEvent(Id));
        }
    }
}

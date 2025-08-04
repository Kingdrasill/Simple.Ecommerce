using Simple.Ecommerce.Domain.Entities.CouponEntity;
using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;

namespace Simple.Ecommerce.Domain.OrderProcessing.Commands
{
    public class ProcessConfirmedNewOrderCommand : ICommand
    {
        public Order Order { get; private set; }
        public string UserName { get; private set; }
        public Coupon? OrderCoupon { get; private set; }
        public Discount? OrderDiscount { get; private set; }
        public List<(OrderItem, string, Coupon?, Discount?)> CompleteOrderItems { get; private set; }

        public ProcessConfirmedNewOrderCommand(Order order, string userName, Coupon? orderCoupon, Discount? orderDiscount, List<(OrderItem, string, Coupon?, Discount?)> completeOrderItems)
        {
            Order = order;
            UserName = userName;
            OrderDiscount = orderDiscount;
            CompleteOrderItems = completeOrderItems;
        }
    }
}

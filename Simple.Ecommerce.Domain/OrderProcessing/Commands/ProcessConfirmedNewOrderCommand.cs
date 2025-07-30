using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;

namespace Simple.Ecommerce.Domain.OrderProcessing.Commands
{
    public class ProcessConfirmedNewOrderCommand : ICommand
    {
        public Order Order { get; private set; }
        public string UserName { get; private set; }
        public Discount? OrderDiscount { get; private set; }
        public List<(OrderItem, string, Discount?)> OrderItemWithDiscounts { get; private set; }

        public ProcessConfirmedNewOrderCommand(Order order, string userName, Discount? orderDiscount, List<(OrderItem, string, Discount?)> orderItemWithDiscounts)
        {
            Order = order;
            UserName = userName;
            OrderDiscount = orderDiscount;
            OrderItemWithDiscounts = orderItemWithDiscounts;
        }
    }
}

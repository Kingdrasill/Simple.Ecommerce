namespace Simple.Ecommerce.Domain.OrderProcessing.Commands
{
    public class RevertOrderCommand : ICommand
    {
        public int OrderId { get; private set; }

        public RevertOrderCommand(int orderId) 
        {
            OrderId = orderId;
        }
    }
}

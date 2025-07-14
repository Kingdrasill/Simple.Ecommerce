namespace Simple.Ecommerce.Domain.OrderProcessing.Commands
{
    public class ProcessConfirmedOrderCommand : ICommand
    {
        public int OrderId { get; private set; }

        public ProcessConfirmedOrderCommand(int orderId)
        {
            OrderId = orderId;
        }   
    }
}

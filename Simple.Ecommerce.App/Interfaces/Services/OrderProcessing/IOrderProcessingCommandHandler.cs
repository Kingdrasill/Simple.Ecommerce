using Simple.Ecommerce.Domain.OrderProcessing.Commands;

namespace Simple.Ecommerce.App.Interfaces.Services.OrderProcessing
{
    public interface IOrderProcessingCommandHandler<TCommand, TResult> where TCommand : ICommand
    {
    }
}

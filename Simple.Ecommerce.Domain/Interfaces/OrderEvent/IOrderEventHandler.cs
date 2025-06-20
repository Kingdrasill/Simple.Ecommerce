namespace Simple.Ecommerce.Domain.Interfaces.OrderEvent
{
    public interface IOrderEventHandler<TEvent>
    {
        Task Handle(TEvent @event);
    }
}

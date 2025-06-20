namespace Simple.Ecommerce.Domain.Interfaces.DeleteEvent
{
    public interface IDeleteEventHandler<TEvent> where TEvent : IDeleteEvent
    {
        Task Handle(TEvent domainEvent);
    }
}
